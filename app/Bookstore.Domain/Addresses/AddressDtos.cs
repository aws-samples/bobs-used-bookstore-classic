namespace Bookstore.Domain.Addresses
{
    public class CreateAddressDto 
    { 
        public CreateAddressDto(
        string AddressLine1,
        string AddressLine2,
        string City,
        string State,
        string Country,
        string ZipCode,
        string CustomerSub)
        {
            this.AddressLine1 = AddressLine1;
            this.AddressLine2 = AddressLine2;
            this.City = City;
            this.State = State;
            this.Country = Country;
            this.ZipCode = ZipCode;
            this.CustomerSub = CustomerSub;
        }

        public string AddressLine1 { get; }
        public string AddressLine2 { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string ZipCode { get; }
        public string CustomerSub { get; }
    }

    public class UpdateAddressDto
    {
        public UpdateAddressDto(
        int AddressId,
        string AddressLine1,
        string AddressLine2,
        string City,
        string State,
        string Country,
        string ZipCode,
        string CustomerSub)
        {
            this.AddressId = AddressId;
            this.AddressLine1 = AddressLine1;
            this.AddressLine2 = AddressLine2;
            this.City = City;
            this.State = State;
            this.Country = Country;
            this.ZipCode = ZipCode;
            this.CustomerSub = CustomerSub;
        }

        public int AddressId { get; }
        public string AddressLine1 { get; }
        public string AddressLine2 { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string ZipCode { get; }
        public string CustomerSub { get; }
    }

    public class DeleteAddressDto
    {
        public DeleteAddressDto(
        int AddressId,
        string CustomerSub)
        {
            this.AddressId = AddressId;
            this.CustomerSub = CustomerSub;
        }

        public int AddressId { get; }
        public string CustomerSub { get; }
    }
}