using Bookstore.Domain.Addresses;
using Bookstore.Domain.Customers;
using Bookstore.Web.Helpers;
using Bookstore.Web.ViewModel.Address;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bookstore.Web.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressService addressService;
        private readonly ICustomerService customerService;

        public AddressController(IAddressService addressService, ICustomerService customerService)
        {
            this.addressService = addressService;
            this.customerService = customerService;
        }

        public async Task<ActionResult> Index()
        {
            var addresses = await addressService.GetAddressesAsync(User.GetSub());

            return View(new AddressIndexViewModel(addresses));
        }

        public ActionResult Create(string returnUrl)
        {
            var model = new AddressCreateUpdateViewModel(returnUrl);

            return View("CreateUpdate", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(AddressCreateUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View("CreateUpdate", model);

            var dto = new CreateAddressDto(model.AddressLine1, model.AddressLine2, model.City, model.State, model.Country, model.ZipCode, User.GetSub());

            await addressService.CreateAddressAsync(dto);

            return Redirect(model.ReturnUrl);
        }

        public async Task<ActionResult> Update(int id, string returnUrl)
        {
            var address = await addressService.GetAddressAsync(User.GetSub(), id);

            return View("CreateUpdate", new AddressCreateUpdateViewModel(address, returnUrl));
        }

        [HttpPost]
        public async Task<ActionResult> Update(AddressCreateUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new UpdateAddressDto(model.Id, model.AddressLine1, model.AddressLine2, model.City, model.State, model.Country, model.ZipCode, User.GetSub());

            await addressService.UpdateAddressAsync(dto);

            return Redirect(model.ReturnUrl);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var dto = new DeleteAddressDto(id, User.GetSub());

            await addressService.DeleteAddressAsync(dto);

            this.SetNotification("Address deleted");

            return RedirectToAction(nameof(Index));
        }
    }
}