using Microsoft.AspNetCore.Owin;
using Microsoft.Owin;
using Owin;
using NLog;
using System;



[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public static class LoggingSetup
    {
        public static void ConfigureLogging()
        {
            // Configure NLog
            try
            {
                var config = new NLog.Config.LoggingConfiguration();

                // Add console target
                var consoleTarget = new NLog.Targets.ConsoleTarget("console");
                config.AddTarget(consoleTarget);
                config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);

                LogManager.Configuration = config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring logging: {ex.Message}");
            }
        }
    }

    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Configure application settings
            try
            {
                // Implementation for configuration setup
                Console.WriteLine("Configuring application settings");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring settings: {ex.Message}");
            }
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // Configure dependency injection
            try
            {
                // Implementation for dependency injection setup
                Console.WriteLine("Configuring dependency injection");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring dependency injection: {ex.Message}");
            }
        }
    }

    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // Configure authentication
            try
            {
                // Implementation for authentication setup
                Console.WriteLine("Configuring authentication");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring authentication: {ex.Message}");
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}