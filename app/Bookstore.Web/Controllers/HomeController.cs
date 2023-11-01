using System.Diagnostics;
using Bookstore.Web.ViewModel;
using Bookstore.Domain.Books;
using System.Threading.Tasks;
using Bookstore.Web.ViewModel.Home;
using System.Web.Mvc;

namespace Bookstore.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IBookService bookService;

        public HomeController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        public async Task<ActionResult> Index()
        {
            var books = await bookService.ListBestSellingBooksAsync(4);

            return View(new HomeIndexViewModel(books));
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Search()
        {
            return RedirectToAction("Index", "Search");
        }

        public ActionResult Cart()
        {
            return RedirectToAction("Index", "ShoppingCart");
        }

        public ActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id });
        }
    }
}
