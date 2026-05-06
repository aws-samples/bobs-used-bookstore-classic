using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using Amazon.Rekognition;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BobsBookstoreClassic.Data;
using Bookstore.Common;
using Bookstore.Data;
using Bookstore.Data.FileServices;
using Bookstore.Data.ImageResizeService;
using Bookstore.Data.ImageValidationServices;
using Bookstore.Data.Repositories;
using Bookstore.Domain;
using Bookstore.Domain.Addresses;
using Bookstore.Domain.Books;
using Bookstore.Domain.Carts;
using Bookstore.Domain.Customers;
using Bookstore.Domain.Offers;
using Bookstore.Domain.Orders;
using Bookstore.Domain.ReferenceData;
using Bookstore.Web.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using NLog.Config;
using NLog.Targets;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// 1. Configuration - load AWS SSM parameters into BookstoreConfiguration when
//    running with aws service flags (mirrors ConfigurationSetup.ConfigureConfiguration)
// ---------------------------------------------------------------------------
ConfigureConfiguration(builder.Configuration);

// ---------------------------------------------------------------------------
// 2. Logging - NLog (mirrors LoggingSetup.ConfigureLogging)
// ---------------------------------------------------------------------------
ConfigureNLog(builder.Configuration);

// ---------------------------------------------------------------------------
// 3. Autofac - keep Autofac as the DI container
//    (mirrors DependencyInjectionSetup.ConfigureDependencyInjection)
// ---------------------------------------------------------------------------
builder.Host.UseNLog();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterType<BookService>().As<IBookService>();
    containerBuilder.RegisterType<OrderService>().As<IOrderService>();
    containerBuilder.RegisterType<ReferenceDataService>().As<IReferenceDataService>();
    containerBuilder.RegisterType<OfferService>().As<IOfferService>();
    containerBuilder.RegisterType<CustomerService>().As<ICustomerService>();
    containerBuilder.RegisterType<AddressService>().As<IAddressService>();
    containerBuilder.RegisterType<ShoppingCartService>().As<IShoppingCartService>();
    containerBuilder.RegisterType<ImageResizeService>().As<IImageResizeService>();

    var connectionString = BookstoreConfiguration.GetConnectionString("BookstoreDatabaseConnection");
    containerBuilder.RegisterType<ApplicationDbContext>()
        .WithParameter("connectionString", connectionString)
        .InstancePerLifetimeScope();

    containerBuilder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
    containerBuilder.RegisterType<AddressRepository>().As<IAddressRepository>();
    containerBuilder.RegisterType<BookRepository>().As<IBookRepository>();
    containerBuilder.RegisterType<OfferRepository>().As<IOfferRepository>();
    containerBuilder.RegisterType<ShoppingCartRepository>().As<IShoppingCartRepository>();
    containerBuilder.RegisterType<OrderRepository>().As<IOrderRepository>();
    containerBuilder.RegisterType<ReferenceDataRepository>().As<IReferenceDataRepository>();

    containerBuilder.RegisterGeneric(typeof(PaginatedList<>))
        .As(typeof(IPaginatedList<>))
        .InstancePerLifetimeScope();

    if (BookstoreConfiguration.GetSetting("Services/FileService") == "aws")
    {
        containerBuilder.RegisterType<AmazonS3Client>().As<IAmazonS3>();
        containerBuilder.RegisterType<S3FileService>().As<IFileService>();
    }
    else
    {
        var webRootPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            "wwwroot",
            "Content");
        containerBuilder.RegisterInstance(new LocalFileService(webRootPath)).As<IFileService>();
    }

    if (BookstoreConfiguration.GetSetting("Services/ImageValidationService") == "aws")
    {
        containerBuilder.RegisterType<AmazonRekognitionClient>().As<IAmazonRekognition>();
        containerBuilder.RegisterType<RekognitionImageValidationService>().As<IImageValidationService>();
    }
    else
    {
        containerBuilder.RegisterType<LocalImageValidationService>().As<IImageValidationService>();
    }

    if (BookstoreConfiguration.GetSetting("Services/Authentication") != "aws")
    {
        containerBuilder.RegisterType<LocalAuthenticationMiddleware>();
    }
});

// ---------------------------------------------------------------------------
// 4. MVC with global filters (mirrors FilterConfig.RegisterGlobalFilters)
//    HandleErrorAttribute  -> built-in exception handling middleware
//    AuthorizeAttribute    -> global AuthorizeFilter applied to all actions
// ---------------------------------------------------------------------------
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// ---------------------------------------------------------------------------
// 5. Authentication (mirrors AuthenticationConfig.ConfigureAuthentication)
//    Branches on Services/Authentication == "aws" for Cognito OIDC,
//    otherwise falls back to cookie-only for LocalAuthenticationMiddleware.
// ---------------------------------------------------------------------------
if (BookstoreConfiguration.GetSetting("Services/Authentication") == "aws")
{
    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect(options =>
        {
            options.ClientId = BookstoreConfiguration.GetSetting("Authentication/Cognito/LocalClientId");
            options.MetadataAddress = BookstoreConfiguration.GetSetting("Authentication/Cognito/MetadataAddress");
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.UsePkce = true;
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.SaveTokens = true;
            options.UseTokenLifetime = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "cognito:username",
                RoleClaimType = "cognito:groups"
            };
            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = context =>
                {
                    var returnUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.Path;
                    context.ProtocolMessage.RedirectUri = returnUrl;
                    return System.Threading.Tasks.Task.CompletedTask;
                },
                OnAuthorizationCodeReceived = context =>
                {
                    var returnUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.Path;
                    context.TokenEndpointRequest.RedirectUri = returnUrl;
                    return System.Threading.Tasks.Task.CompletedTask;
                },
                OnTokenValidated = async context =>
                {
                    var customerService = context.HttpContext.RequestServices
                        .GetRequiredService<ICustomerService>();

                    var identity = (ClaimsIdentity)context.Principal.Identity;

                    var dto = new CreateOrUpdateCustomerDto(
                        identity.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value ?? string.Empty,
                        identity.Name ?? string.Empty,
                        identity.FindFirst(c => c.Type.Contains("givenname"))?.Value ?? string.Empty,
                        identity.FindFirst(c => c.Type.Contains("surname"))?.Value ?? string.Empty);

                    await customerService.CreateOrUpdateCustomerAsync(dto);
                }
            };
        });
}
else
{
    // Local authentication - cookie scheme used by LocalAuthenticationMiddleware
    builder.Services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie();
}

// ---------------------------------------------------------------------------
// Build the application
// ---------------------------------------------------------------------------
var app = builder.Build();

// ---------------------------------------------------------------------------
// 6. Exception / error handling middleware
//    (mirrors FilterConfig HandleErrorAttribute and Startup exception handler)
// ---------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ---------------------------------------------------------------------------
// 7. Static files
//    BundleConfig (System.Web.Optimization) is not supported in ASP.NET Core;
//    static assets are served directly from wwwroot.
// ---------------------------------------------------------------------------
app.UseStaticFiles();

app.UseRouting();

// ---------------------------------------------------------------------------
// 8. Authentication / Authorization middleware
// ---------------------------------------------------------------------------
app.UseAuthentication();

if (BookstoreConfiguration.GetSetting("Services/Authentication") != "aws")
{
    // Local dev authentication middleware replaces OWIN UseMiddlewareFromContainer
    app.UseMiddleware<LocalAuthenticationMiddleware>();
}

app.UseAuthorization();

// ---------------------------------------------------------------------------
// 9. Route registration
//    Area route (mirrors AdminAreaRegistration.RegisterArea)
//    Default route (mirrors RouteConfig.RegisterRoutes)
// ---------------------------------------------------------------------------
app.MapAreaControllerRoute(
    name: "Admin_default",
    areaName: "Admin",
    pattern: "Admin/{controller}/{action}/{id?}",
    defaults: new { action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ---------------------------------------------------------------------------
// Local helper: ConfigureConfiguration
// Mirrors ConfigurationSetup.ConfigureConfiguration - loads AWS SSM parameters
// into BookstoreConfiguration when running with aws service flags.
// ---------------------------------------------------------------------------
static void ConfigureConfiguration(IConfiguration configuration)
{
    var rootPath = "/" + Constants.AppName;
    const string databasePath = "/Database";
    const string authenticationPath = "/Authentication";
    const string fileServicePath = "/Files";

    if (configuration["Services/Database"] == "aws")
    {
        using var client = new AmazonSimpleSystemsManagementClient();
        var request = new GetParameterRequest
        {
            Name = $"{rootPath}{databasePath}/ConnectionStrings/BookstoreDatabaseConnection"
        };
        var response = client.GetParameterAsync(request).Result;
        BookstoreConfiguration.AddSetting(
            response.Parameter.Name.Replace($"{rootPath}{databasePath}/", string.Empty),
            response.Parameter.Value);
    }

    if (configuration["Services/Authentication"] == "aws")
    {
        using var client = new AmazonSimpleSystemsManagementClient();
        var request = new GetParametersByPathRequest
        {
            Path = $"{rootPath}{authenticationPath}/",
            Recursive = true
        };
        var response = client.GetParametersByPathAsync(request).Result;
        foreach (var parameter in response.Parameters)
        {
            BookstoreConfiguration.AddSetting(
                parameter.Name.Replace($"{rootPath}/", string.Empty),
                parameter.Value);
        }
    }

    if (configuration["Services/FileService"] == "aws")
    {
        using var client = new AmazonSimpleSystemsManagementClient();
        var request = new GetParametersByPathRequest
        {
            Path = $"{rootPath}{fileServicePath}/",
            Recursive = true
        };
        var response = client.GetParametersByPathAsync(request).Result;
        foreach (var parameter in response.Parameters)
        {
            BookstoreConfiguration.AddSetting(
                parameter.Name.Replace($"{rootPath}/", string.Empty),
                parameter.Value);
        }
    }
}

// ---------------------------------------------------------------------------
// Local helper: ConfigureNLog
// Mirrors LoggingSetup.ConfigureLogging - configures NLog with AWSTarget or
// DebuggerTarget depending on the Services/LoggingService setting.
// ---------------------------------------------------------------------------
static void ConfigureNLog(IConfiguration configuration)
{
    var config = new LoggingConfiguration();

    NLog.Targets.Target loggingTarget;

    if (configuration["Services/LoggingService"] == "aws")
    {
        loggingTarget = new DebuggerTarget("aws-cloudwatch") { };
    }
    else
    {
        loggingTarget = new DebuggerTarget();
    }

    config.AddTarget("aws", loggingTarget);
    config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Info, loggingTarget));

    LogManager.Configuration = config;
}
