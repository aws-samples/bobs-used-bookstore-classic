using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Books;
using Bookstore.Domain.Carts;
using Bookstore.Web.ViewModel.Search;
using Microsoft.AspNetCore.Mvc;


namespace Bookstore.Web.Controllers
{
    [AllowAnonymous]
    public class SearchController : Controller
    {
        private readonly IBookService inventoryService;
        private readonly IShoppingCartService shoppingCartService;

        public SearchController(IBookService inventoryService, IShoppingCartService shoppingCartService)
        {
            this.inventoryService = inventoryService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<ActionResult> Index(string searchString, string sortBy = "Name", int pageIndex = 1, int pageSize = 10)
        {
            var books = await inventoryService.GetBooksAsync(searchString, sortBy, pageIndex, pageSize);

            return View(new SearchIndexViewModel(books));
        }

        public async Task<ActionResult> Details(int id)
        {
            var book = await inventoryService.GetBookAsync(id);

            return View(new SearchDetailsViewModel(book));
        }

        public async Task<ActionResult> AddItemToShoppingCart(int bookId)
        {
            var dto = new AddToShoppingCartDto(HttpContext.GetShoppingCartCorrelationId(), bookId, 1);

            await shoppingCartService.AddToShoppingCartAsync(dto);

            this.SetNotification("Item added to shopping cart");

            return RedirectToAction("Index", "Search");
        }

        public async Task<ActionResult> AddItemToWishlist(int bookId)
        {
            var dto = new AddToWishlistDto(HttpContext.GetShoppingCartCorrelationId(), bookId);

            await shoppingCartService.AddToWishlistAsync(dto);

            this.SetNotification("Item added to wishlist");

            return RedirectToAction("Index", "Search");
        }
    }
}
