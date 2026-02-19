
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
using NLog;


    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var logger = NLog.LogManager.Setup().GetCurrentClassLogger();
                try
                {
                    var builder = WebApplication.CreateBuilder(args);

                    // Store configuration in static ConfigurationManager
                    ConfigurationManager.Configuration = builder.Configuration;

                    // Add connection string configuration
                    var connectionString = builder.Configuration.GetConnectionString("BookstoreDatabaseConnection");

                    // Configure NLog for Dependency Injection
                    builder.Logging.ClearProviders();
                    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);


                    // Add services to the container (formerly ConfigureServices)
                    builder.Services.AddControllersWithViews();
                    //Added Services

                    var app = builder.Build();

                    // Global error handling middleware (replaces Application_Error)
                    app.Use(async (context, next) =>
                    {
                        try
                        {
                            await next();
                        }
                        catch (Exception ex)
                        {
                            var errorLogger = app.Services.GetRequiredService<ILogger<Program>>();
                            errorLogger.LogError(ex, "Unhandled exception occurred");
                            throw;
                        }
                    });

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

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Stopped program because of exception");
                    throw;
                }
                finally
                {
                    NLog.LogManager.Shutdown();
                }
            }
        }

        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }
