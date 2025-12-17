using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bookstore.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration is handled through IConfiguration in ASP.NET Core
            // ConfigurationSetup.ConfigureConfiguration();

            // Add service registrations here
            // DependencyInjectionSetup.ConfigureDependencyInjection(services);
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
// Authentication should be configured in ConfigureServices using services.AddAuthentication()
// and the middleware should be added here using app.UseAuthentication() and app.UseAuthorization()
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
