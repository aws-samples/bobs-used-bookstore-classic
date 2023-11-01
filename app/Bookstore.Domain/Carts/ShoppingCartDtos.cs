namespace Bookstore.Domain.Carts
{
    public class AddToShoppingCartDto
    {
        public AddToShoppingCartDto(string CorrelationId, int BookId, int Quantity)
        {
            this.CorrelationId = CorrelationId;
            this.BookId = BookId;
            this.Quantity = Quantity;
        }

        public string CorrelationId { get; }
        public int BookId { get; }
        public int Quantity { get; }
    }

    public class AddToWishlistDto
    {
        public AddToWishlistDto(string CorrelationId, int BookId)
        {
            this.CorrelationId = CorrelationId;
            this.BookId = BookId;
        }

        public string CorrelationId { get; }
        public int BookId { get; }
    }

    public class MoveWishlistItemToShoppingCartDto
    {
        public MoveWishlistItemToShoppingCartDto(string CorrelationId, int ShoppingCartItemId)
        {
            this.CorrelationId = CorrelationId;
            this.ShoppingCartItemId = ShoppingCartItemId;
        }

        public string CorrelationId { get; }
        public int ShoppingCartItemId { get; }
    }

    public class MoveAllWishlistItemsToShoppingCartDto
    {
        public MoveAllWishlistItemsToShoppingCartDto(string CorrelationId)
        {
            this.CorrelationId = CorrelationId;
        }

        public string CorrelationId { get; }
    }

    public class DeleteShoppingCartItemDto
    {
        public DeleteShoppingCartItemDto(string CorrelationId, int ShoppingCartItemId)
        {
            this.CorrelationId = CorrelationId;
            this.ShoppingCartItemId = ShoppingCartItemId;
        }

        public string CorrelationId { get; }
        public int ShoppingCartItemId { get; }
    }
}
