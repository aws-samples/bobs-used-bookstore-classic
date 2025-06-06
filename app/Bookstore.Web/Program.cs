
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
    
    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add connection string from Web.config
                builder.Configuration.GetSection("ConnectionStrings").GetSection("BookstoreDatabaseConnection").Value =
                    "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;";

                // Add app settings from Web.config
                builder.Configuration["ClientValidationEnabled"] = "true";
                builder.Configuration["Environment"] = "Development";
                builder.Configuration["Services:Authentication"] = "local";
                builder.Configuration["Services:Database"] = "local";
                builder.Configuration["Services:FileService"] = "local";
                builder.Configuration["Services:ImageValidationService"] = "local";
                builder.Configuration["Services:LoggingService"] = "local";
                builder.Configuration["Authentication:Cognito:LocalClientId"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
                builder.Configuration["Authentication:Cognito:AppRunnerClientId"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
                builder.Configuration["Authentication:Cognito:MetadataAddress"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
                builder.Configuration["Authentication:Cognito:CognitoDomain"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
                builder.Configuration["Files:BucketName"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']";
                builder.Configuration["Files:CloudFrontDomain"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']";

                // Entity Framework 6 configuration
                // EF6 is configured via the connection string in the app.config/web.config
                // and doesn't use ASP.NET Core's dependency injection system directly

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();
                builder.Services.AddRazorPages();

                // Add MVC areas support
                builder.Services.AddMvc()
                    .AddMvcOptions(options => {
                        // Add filters here if needed
                    });

                // Configure bundling and minification
                // In .NET Core, this is typically handled by tools like WebOptimizer
// or by using bundling and minification in your build process
                
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

                // Configure logging
                var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();

                app.UseExceptionHandler(errorApp => {
                    errorApp.Run(async context => {
                        // Log the exception
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        if (exceptionHandlerPathFeature?.Error != null) {
                            logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception");
                        }
                        await Task.CompletedTask;
                    });
                });

                app.UseRouting();
                
                app.UseAuthorization();
                
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.MapAreaControllerRoute(
                    name: "areas",
                    areaName: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                app.MapRazorPages();
                
                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }