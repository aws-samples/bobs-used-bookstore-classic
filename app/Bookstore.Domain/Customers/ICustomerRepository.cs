using System.Threading.Tasks;

namespace Bookstore.Domain.Customers
{
    public interface ICustomerRepository
    {
        Task<Customer> GetAsync(int id);

        Task<Customer> GetAsync(string sub);

        Task AddAsync(Customer customer);

        Task SaveChangesAsync();
    }
}