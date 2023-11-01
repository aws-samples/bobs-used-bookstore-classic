namespace Bookstore.Domain.Orders
{
    public class CreateOrderDto
    {
        public CreateOrderDto(string CustomerSub, string CorrelationId, int AddressId)
        {
            this.CustomerSub = CustomerSub;
            this.CorrelationId = CorrelationId;
            this.AddressId = AddressId;
        }

        public string CustomerSub { get; }
        public string CorrelationId { get; }
        public int AddressId { get; }
    }

    public class UpdateOrderStatusDto
    {
        public UpdateOrderStatusDto(int OrderId, OrderStatus OrderStatus)
        {
            this.OrderId = OrderId;
            this.OrderStatus = OrderStatus;
        }

        public int OrderId { get; }
        public OrderStatus OrderStatus { get; }
    }

    public class CancelOrderDto
    {
        public CancelOrderDto(string CustomerSub, int OrderId)
        {
            this.CustomerSub = CustomerSub;
            this.OrderId = OrderId;
        }

        public string CustomerSub { get; }
        public int OrderId { get; }
    }
}