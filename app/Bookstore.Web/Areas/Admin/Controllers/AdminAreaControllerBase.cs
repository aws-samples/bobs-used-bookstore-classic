
using System.Web.Mvc;

namespace Bookstore.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [Authorize(Roles = "Administrators")]
    public abstract class AdminAreaControllerBase : Controller { }
}