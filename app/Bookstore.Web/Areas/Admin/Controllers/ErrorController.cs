using Microsoft.AspNetCore.Mvc;


namespace Bookstore.Web.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class ErrorController : AdminAreaControllerBase
    {
        [Route("/Error/Index/{code:int}")]
        public ActionResult Index(int code)
        {
            //var exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            //ViewData["Path"] = exception?.Path;
            //ViewData["StatusCode"] = code;
            return View();
        }

        [Route("/error")]
        public ActionResult Support()
        {
            //var exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            //ViewData["Path"] = exception?.Path;
            //var error = Problem();

            //ViewData["StatusCode"] = error.StatusCode;
            return View("~/Views/Error/Index.cshtml");
        }
    }
}