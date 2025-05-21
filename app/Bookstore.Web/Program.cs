
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bookstore.Web;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);
                
                // Add connection string from web.config
                builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
                builder.Configuration.AddEnvironmentVariables();

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Register client validation and other settings from appSettings
                builder.Services.AddMvc()
                    .AddMvcOptions(options =>
                    {
                        // Register global filters here (formerly FilterConfig.RegisterGlobalFilters)
                    })
                    .AddViewOptions(options =>
                    {
                        options.HtmlHelperOptions.ClientValidationEnabled =
                            bool.Parse(builder.Configuration["ClientValidationEnabled"] ?? "true");
                    });

                // Register bundles
                // Note: In ASP.NET Core, bundling is typically handled by other mechanisms like WebPack or Bundler & Minifier

                // Configure logging
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();
                builder.Logging.AddEventSourceLogger();

                // Configure Entity Framework (not EF Core)
                SqlProviderServices.SqlServerTypesAssemblyName = typeof(SqlProviderServices).Assembly.FullName;

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

                // Configure error handling (formerly Application_Error)
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        logger.LogError(exception, "Unhandled exception");

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected error occurred.");
                    });
                });

                app.UseRouting();
                
                app.UseAuthorization();
                
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }