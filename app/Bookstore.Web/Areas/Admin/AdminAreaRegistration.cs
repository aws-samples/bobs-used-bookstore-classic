using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;

namespace Bookstore.Web.Areas
{
    public class AdminAreaRegistration
    {
        public string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public void RegisterArea(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin_default",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}