
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
using Microsoft.AspNetCore.Routing;
using EntityFramework = System.Data.Entity;
using WebOptimizer;

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add connection string from Web.config
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"ConnectionStrings:BookstoreDatabaseConnection", "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;"}
                });

                // Add app settings from Web.config
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"webpages:Version", "3.0.0.0"},
                    {"webpages:Enabled", "false"},
                    {"ClientValidationEnabled", "true"},
                    {"Environment", "Development"},
                    {"Services:Authentication", "local"},
                    {"Services:Database", "local"},
                    {"Services:FileService", "local"},
                    {"Services:ImageValidationService", "local"},
                    {"Services:LoggingService", "local"},
                    {"Authentication:Cognito:LocalClientId", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']"},
                    {"Authentication:Cognito:AppRunnerClientId", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']"},
                    {"Authentication:Cognito:MetadataAddress", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']"},
                    {"Authentication:Cognito:CognitoDomain", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']"},
                    {"Files:BucketName", "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']"},
                    {"Files:CloudFrontDomain", "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']"}
                });

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;
                
                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews(options => {
                    options.EnableEndpointRouting = true;
                });

                // Client validation settings from Web.config
                if (builder.Configuration["ClientValidationEnabled"] == "true")
                {
                    builder.Services.AddMvc().AddDataAnnotationsLocalization();
                }

                // Register global filters (from FilterConfig)
                builder.Services.AddMvc(options =>
                {
                    // Add any global filters here if needed
                });

                // Add Entity Framework services (from Web.config entityFramework section)
                builder.Services.AddScoped<EntityFramework.DbContext>();

                // Add bundling services - can use WebOptimizer for .NET Core
                builder.Services.AddWebOptimizer(pipeline =>
                {
                    // Configure bundles here if needed
                });

                // Areas are supported by default with AddControllersWithViews or AddMvc

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

                // WebOptimizer middleware (replacement for BundleConfig)
                app.UseWebOptimizer();

                app.UseRouting();

                app.UseAuthorization();
                
                // Register routes (from RouteConfig)
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Register area routes if needed
                app.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                // Configure logging
                var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        if (exceptionHandlerPathFeature?.Error != null)
                        {
                            logger.LogError(exceptionHandlerPathFeature.Error, "An unhandled exception occurred");
                        }
                        await Task.CompletedTask;
                    });
                });

                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }