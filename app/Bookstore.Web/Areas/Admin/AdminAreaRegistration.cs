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
            app.UseEndpoints(endpoints => {
                endpoints.MapAreaControllerRoute(
                    "Admin_default",
                    "Admin",
                    "Admin/{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}