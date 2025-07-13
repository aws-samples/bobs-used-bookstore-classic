using Microsoft.AspNetCore.Owin;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using NLog.Extensions.Logging;
using Owin;


[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Configuration setup logic here
        }
    }

    public class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // Dependency injection setup logic here
        }
    }

    public class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // Authentication configuration logic here
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }

        private void ConfigureLogging()
        {
            // Configure NLog for logging
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddNLog();
            });

            // Optional: You can get a logger instance here if needed
            // var logger = loggerFactory.CreateLogger<Startup>();
        }
    }
}