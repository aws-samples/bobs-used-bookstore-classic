
using Amazon.Rekognition;
using Amazon.S3;
using Autofac.Core;
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
    
    
namespace Bookstore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
    
            // Add configuration sources to match Web.config settings
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();
    
            // Add connection strings from Web.config
            var connectionString = builder.Configuration.GetConnectionString("BookstoreDatabaseConnection");
    
    
            builder.Services.AddTransient<ApplicationDbContext>(s => new ApplicationDbContext(connectionString));
    
    
            AddServices(builder.Services, builder);
    
            // Add services to the container (formerly ConfigureServices)
            builder.Services.AddControllersWithViews()
                .AddMvcOptions(options =>
                {
                    options.EnableEndpointRouting = true;
                });
    
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
    
                // Configure logging service based on settings
                var loggingService = builder.Configuration["Services:LoggingService"];
                if (loggingService == "local")
                {
                    // Local logging configuration
                }
            });
    
            // Add filters equivalent to FilterConfig.RegisterGlobalFilters
            builder.Services.AddMvc(options =>
            {
                // Add global filters here if needed
            });
    
            // Client validation settings from Web.config
            builder.Services.AddRazorPages().AddViewOptions(options =>
            {
                options.HtmlHelperOptions.ClientValidationEnabled =
                    bool.TryParse(builder.Configuration["ClientValidationEnabled"], out bool clientValidationEnabled)
                    ? clientValidationEnabled : true;
            });
    
            // Configure service options based on appSettings
            var environment = builder.Configuration["Environment"] ?? "Development";
            var authService = builder.Configuration["Services:Authentication"] ?? "local";
            var databaseService = builder.Configuration["Services:Database"] ?? "local";
            var fileService = builder.Configuration["Services:FileService"] ?? "local";
            var imageValidationService = builder.Configuration["Services:ImageValidationService"] ?? "local";
    
            // Configure authentication based on settings
            if (authService == "aws")
            {
                // AWS Cognito Authentication settings would be configured here
            }
    
            // File service configuration based on settings
            if (fileService == "aws")
            {
                // AWS File Service settings would be configured here
            }
    
            //Added Services
    
            // Store configuration in static ConfigurationManager
            ConfigurationManager.Configuration = builder.Configuration;
    
            var app = builder.Build();
    
            // Configure the HTTP request pipeline (formerly Configure method)
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
    
            app.UseHttpsRedirection();
            app.UseStaticFiles();
    
            // Register areas equivalent to AreaRegistration.RegisterAllAreas()
    
            // Exception handling
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    var logger = app.Services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An unhandled exception occurred");
                    throw;
                }
            });
    
            //Added Middleware
    
            app.UseRouting();
    
            app.UseAuthorization();
    
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
    
            // Additional routes can be configured here if needed
            // This is equivalent to RouteConfig.RegisterRoutes
    
            app.Run();
        }
    
        private static void AddServices(IServiceCollection services, WebApplicationBuilder builder)
        {
    
            services.AddControllersWithViews();
            services.AddTransient<IBookService, BookService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IReferenceDataService, ReferenceDataService>();
            services.AddTransient<IOfferService, OfferService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IAddressService, AddressService>();
            services.AddTransient<IShoppingCartService, ShoppingCartService>();
            services.AddTransient<IImageResizeService, ImageResizeService>();
    
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IAddressRepository, AddressRepository>();
            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<IOfferRepository, OfferRepository>();
            services.AddTransient<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IReferenceDataRepository, ReferenceDataRepository>();
    
            // File Service
            if (builder.Configuration["Services:FileService"] == "aws")
            {
                services.AddTransient<IAmazonS3, AmazonS3Client>();
                services.AddTransient<IFileService, S3FileService>();
            }
            else
            {
                var webRootPath = GetWebRootPath();
                services.AddTransient<IFileService, LocalFileService>(s => new LocalFileService(webRootPath));
            }
    
            // Image Validation Service
            if (builder.Configuration["Services:ImageValidationService"] == "aws")
            {
                services.AddTransient<IAmazonRekognition, AmazonRekognitionClient>();
                services.AddTransient<IImageValidationService, RekognitionImageValidationService>();
            }
            else
            {
                services.AddTransient<IImageValidationService, LocalImageValidationService>();
            }
    
            // Authentication
            if (builder.Configuration["Services:Authentication"] != "aws")
            {
                //services.AddTransient<LocalAuthenticationMiddleware>();
            }
    
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            string GetWebRootPath()
            {
                if (builder.Environment.WebRootPath != null)
                {
                    // If running in a web context
                    return Path.Combine(builder.Environment.WebRootPath, "Content");
                }
                else
                {
                    // If running in a non-web context (e.g., console application)
                    return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }
            }
        }
    }
}
    
public class ConfigurationManager
{
    public static IConfiguration Configuration { get; set; }
}