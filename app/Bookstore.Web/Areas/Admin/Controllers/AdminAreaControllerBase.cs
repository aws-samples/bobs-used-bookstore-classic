using Microsoft.AspNetCore.Mvc;


namespace Bookstore.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [Authorize(Roles = "Administrators")]
    public abstract class AdminAreaControllerBase : Controller { }
}