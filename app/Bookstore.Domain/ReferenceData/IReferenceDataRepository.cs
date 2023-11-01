using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookstore.Domain.ReferenceData
{
    public interface IReferenceDataRepository
    {
        Task<IEnumerable<ReferenceDataItem>> FullListAsync();

        Task<IPaginatedList<ReferenceDataItem>> ListAsync(ReferenceDataFilters filters, int pageIndex, int pageSize);

        Task<ReferenceDataItem> GetAsync(int id);

        Task AddAsync(ReferenceDataItem item);

        Task SaveChangesAsync();
    }
}
