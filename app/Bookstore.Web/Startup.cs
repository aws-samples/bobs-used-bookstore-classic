using Microsoft.AspNetCore.Owin;
using Microsoft.Owin;
using Owin;



[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // TODO: Add configuration setup logic for .NET 8
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // TODO: Add dependency injection configuration logic
        }
    }

    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // TODO: Add authentication configuration logic
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // TODO: Add proper logging setup
            // LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}