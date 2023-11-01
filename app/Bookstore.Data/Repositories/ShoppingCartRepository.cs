using Bookstore.Domain.Carts;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace Bookstore.Data.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task IShoppingCartRepository.AddAsync(ShoppingCart shoppingCart)
        {
            await Task.Run(() => dbContext.ShoppingCart.Add(shoppingCart));
        }

        async Task<ShoppingCart> IShoppingCartRepository.GetAsync(string correlationId)
        {
            return await dbContext.ShoppingCart
                .Include(x => x.ShoppingCartItems)
                .Include(x => x.ShoppingCartItems.Select(y => y.Book))
                .SingleOrDefaultAsync(x => x.CorrelationId == correlationId);
        }

        async Task IShoppingCartRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
