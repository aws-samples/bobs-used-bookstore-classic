using Bookstore.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookstore.Domain.Offers
{
    public interface IOfferRepository
    {
        Task<IPaginatedList<Offer>> ListAsync(OfferFilters filters, int pageIndex, int pageSize);

        Task<IEnumerable<Offer>> ListAsync(string sub);

        Task<Offer> GetAsync(int id);

        Task AddAsync(Offer offer);

        Task SaveChangesAsync();

        Task<OfferStatistics> GetStatisticsAsync();
    }
}
