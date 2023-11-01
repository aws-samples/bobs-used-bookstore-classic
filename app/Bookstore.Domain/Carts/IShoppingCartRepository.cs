using System.Threading.Tasks;

namespace Bookstore.Domain.Carts
{
    public interface IShoppingCartRepository
    {
        Task AddAsync(ShoppingCart shoppingCart);

        Task<ShoppingCart> GetAsync(string correlationId);

        Task SaveChangesAsync();
    }
}
