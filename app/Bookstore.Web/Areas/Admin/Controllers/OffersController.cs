using System.Threading.Tasks;
using System.Web.Mvc;
using Bookstore.Domain.Offers;
using Bookstore.Domain.ReferenceData;
using Bookstore.Web.Areas.Admin.Models.Offers;

namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class OffersController : AdminAreaControllerBase
    {
        private readonly IOfferService offerService;
        private readonly IReferenceDataService referenceDataService;

        public OffersController(IOfferService offerService, IReferenceDataService referenceDataService)
        {
            this.offerService = offerService;
            this.referenceDataService = referenceDataService;
        }

        public async Task<ActionResult> Index(OfferFilters filters, int pageIndex = 1, int pageSize = 10)
        {
            var offers = await offerService.GetOffersAsync(filters, pageIndex, pageSize);
            var referenceData = await referenceDataService.GetAllReferenceDataAsync();

            return View(new OfferIndexViewModel(offers, referenceData));
        }

        [HttpPost]
        public async Task<ActionResult> ApproveAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Approved, "The offer has been approved");
        }

        [HttpPost]
        public async Task<ActionResult> RejectAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Rejected, "The offer has been rejected");
        }

        [HttpPost]
        public async Task<ActionResult> ReceivedAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Received, "The book has been received");
        }

        [HttpPost]
        public async Task<ActionResult> PaidAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Paid, "The customer has been paid");
        }

        private async Task<ActionResult> UpdateOfferStatus(int id, OfferStatus status, string message)
        {
            var dto = new UpdateOfferStatusDto(id, status);

            await offerService.UpdateOfferStatusAsync(dto);

            TempData["Message"] = message;

            return RedirectToAction("Index");
        }
    }
}