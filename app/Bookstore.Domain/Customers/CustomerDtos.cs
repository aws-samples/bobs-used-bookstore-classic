namespace Bookstore.Domain.Customers
{
    public class CreateOrUpdateCustomerDto
    {
        public CreateOrUpdateCustomerDto(
        string CustomerSub,
        string Username,
        string FirstName,
        string LastName)
        {
            this.CustomerSub = CustomerSub;
            this.Username = Username;
            this.FirstName = FirstName;
            this.LastName = LastName;
        }

        public string CustomerSub { get; }
        public string Username { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}
