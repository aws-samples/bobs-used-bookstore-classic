using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Customers;
using Bookstore.Domain.Carts;
using Bookstore.Web.ViewModel.Wishlist;
using Microsoft.AspNetCore.Mvc;


namespace Bookstore.Web.Controllers
{
    [AllowAnonymous]
    public class WishlistController : Controller
    {
        private readonly ICustomerService customerService;
        private readonly IShoppingCartService shoppingCartService;

        public WishlistController(ICustomerService customerService, IShoppingCartService shoppingCartService)
        {
            this.customerService = customerService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<ActionResult> Index()
        {
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(HttpContext.GetShoppingCartCorrelationId());

            return View(new WishlistIndexViewModel(shoppingCart));
        }

        [HttpPost]
        public async Task<ActionResult> MoveToShoppingCart(int shoppingCartItemId)
        {
            var dto = new MoveWishlistItemToShoppingCartDto(HttpContext.GetShoppingCartCorrelationId(), shoppingCartItemId);

            await shoppingCartService.MoveWishlistItemToShoppingCartAsync(dto);

            this.SetNotification("Item moved to shopping cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> MoveAllItemsToShoppingCart()
        {
            var dto = new MoveAllWishlistItemsToShoppingCartDto(HttpContext.GetShoppingCartCorrelationId());

            await shoppingCartService.MoveAllWishlistItemsToShoppingCartAsync(dto);

            this.SetNotification("All items moved to shopping cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int shoppingCartItemId)
        {
            var dto = new DeleteShoppingCartItemDto(HttpContext.GetShoppingCartCorrelationId(), shoppingCartItemId);

            await shoppingCartService.DeleteShoppingCartItemAsync(dto);

            this.SetNotification("Item removed from wishlist");

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
