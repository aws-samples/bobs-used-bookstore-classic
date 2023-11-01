using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookstore.Domain.Addresses
{
    public interface IAddressRepository
    {
        Task<Address> GetAsync(string sub, int id);

        Task<IEnumerable<Address>> ListAsync(string sub);

        Task AddAsync(Address address);

        Task DeleteAsync(string sub, int id);

        Task SaveChangesAsync();
    }
}