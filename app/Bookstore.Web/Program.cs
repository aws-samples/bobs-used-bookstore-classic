
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using WebOptimizer;

    namespace Bookstore.Web
    {
public class Program
        {
            // Entity Framework configuration
            private static void ConfigureEntityFramework(IServiceCollection services)
            {
                // Ensure EntityFramework is properly configured
                // This replaces the entityFramework section in Web.config
            }
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Configure Entity Framework 6 connection string
                System.Data.Entity.Database.SetInitializer(new System.Data.Entity.CreateDatabaseIfNotExists<System.Data.Entity.DbContext>());
                // EF6 uses connection strings from configuration directly
                // No need to register DbContext with DI container as it's handled by EF6

                // Add application settings from Web.config
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string> {
                    { "Environment", "Development" },
                    { "Services:Authentication", "local" },
                    { "Services:Database", "local" },
                    { "Services:FileService", "local" },
                    { "Services:ImageValidationService", "local" },
                    { "Services:LoggingService", "local" },
                    { "Authentication:Cognito:LocalClientId", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']" },
                    { "Authentication:Cognito:AppRunnerClientId", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']" },
                    { "Authentication:Cognito:MetadataAddress", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']" },
                    { "Authentication:Cognito:CognitoDomain", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']" },
                    { "Files:BucketName", "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']" },
                    { "Files:CloudFrontDomain", "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']" }
                });

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                var mvcBuilder = builder.Services.AddControllersWithViews(options => {
                    options.Filters.Add(new CustomHandleErrorAttribute());
                });

                // Configure client validation options (was in Web.config appSettings)
                builder.Services.Configure<MvcViewOptions>(options => {
                    options.HtmlHelperOptions.ClientValidationEnabled = true;
                });

                builder.Services.AddRazorPages();

                // Register areas
                builder.Services.Configure<RouteOptions>(options => {
                    options.LowercaseUrls = true;
                    options.AppendTrailingSlash = true;
                });

                // Add bundling
                builder.Services.AddWebOptimizer();

                // Configure logging with NLog
                builder.Logging.AddNLog();
                //Added Services

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
                
                //Added Middleware
                
                app.UseRouting();
                
                app.UseAuthorization();

                // Register routes (similar to RouteConfig.RegisterRoutes)
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Map area routes
                app.MapAreaControllerRoute(
                    name: "areas",
                    areaName: "{area}",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                app.MapRazorPages();

                // Register bundles (similar to BundleConfig.RegisterBundles)
                app.UseWebOptimizer();
                
                app.Run();
            }
    }

    public class ConfigurationManager
    {
        public static IConfiguration Configuration { get; set; }
    }

    // Custom error handler to replace HandleErrorAttribute
    public class CustomHandleErrorAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            // Handle the exception - redirect to error view or perform custom error handling
            if (!context.ExceptionHandled)
            {
                context.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/Error.cshtml"
                };
                context.ExceptionHandled = true;
            }
            base.OnException(context);
        }
    }

    // This class contains filter registrations similar to FilterConfig.RegisterGlobalFilters
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddMvcOptions(options =>
            {
                options.Filters.Add(new CustomHandleErrorAttribute());
            });
        }
    }
}