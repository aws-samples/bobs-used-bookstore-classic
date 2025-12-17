
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
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Configure connection strings for Entity Framework 6
                var connectionString = builder.Configuration.GetConnectionString("BookstoreDatabaseConnection");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    System.Data.Entity.Database.DefaultConnectionFactory =
                        new System.Data.Entity.Infrastructure.SqlConnectionFactory(connectionString);
                }

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Register areas
                builder.Services.AddMvc().AddControllersAsServices();

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

                // Global error logging middleware (replaces Application_Error from Global.asax.cs)
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        if (exception != null)
                        {
                            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                            logger.LogError(exception, "Unhandled exception occurred");
                        }

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An error occurred");
                    });
                });

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
        }

        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }