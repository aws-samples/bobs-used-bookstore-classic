using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Web.Areas
{
    public static class AdminAreaRegistration
    {
        public static void ConfigureAdminArea(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin_default",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}