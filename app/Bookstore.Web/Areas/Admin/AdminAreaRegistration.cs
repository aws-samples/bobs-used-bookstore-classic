using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;


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
                endpoints.MapAreaControllerRoute("Admin_default", "Admin_default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
