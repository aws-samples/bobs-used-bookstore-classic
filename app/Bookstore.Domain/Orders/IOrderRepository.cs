using Bookstore.Domain.Books;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookstore.Domain.Orders
{
    public interface IOrderRepository
    {
        Task<Order> GetAsync(int id);

        Task<Order> GetAsync(int id, string sub);

        Task<IEnumerable<Book>> ListBestSellingBooksAsync(int count);

        Task<IPaginatedList<Order>> ListAsync(OrderFilters filters, int pageIndex = 1, int pageSize = 10);

        Task<IEnumerable<Order>> ListAsync(string sub);

        Task AddAsync(Order order);

        Task<OrderStatistics> GetStatisticsAsync();

        Task SaveChangesAsync();        
    }
}
