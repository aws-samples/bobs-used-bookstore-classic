using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Customers;
using Bookstore.Domain.Carts;
using Bookstore.Web.ViewModel.ShoppingCart;
using System.Web.Mvc;

namespace Bookstore.Web.Controllers
{
    [AllowAnonymous]
    public class ShoppingCartController : Controller
    {
        private readonly ICustomerService customerService;
        private readonly IShoppingCartService shoppingCartService;

        public ShoppingCartController(ICustomerService customerService, IShoppingCartService shoppingCartService)
        {
            this.customerService = customerService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<ActionResult> Index()
        {
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(HttpContext.GetShoppingCartCorrelationId());

            return View(new ShoppingCartIndexViewModel(shoppingCart));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int shoppingCartItemId)
        {
            var dto = new DeleteShoppingCartItemDto(HttpContext.GetShoppingCartCorrelationId(), shoppingCartItemId);

            await shoppingCartService.DeleteShoppingCartItemAsync(dto);

            this.SetNotification("Item removed from shopping cart.");

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
