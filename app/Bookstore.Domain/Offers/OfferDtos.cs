namespace Bookstore.Domain.Offers
{
    public class CreateOfferDto
    {
        public CreateOfferDto(
        string CustomerSub,
        string BookName,
        string Author,
        string ISBN,
        int BookTypeId,
        int ConditionId,
        int GenreId,
        int PublisherId,
        decimal BookPrice)
        {
            this.CustomerSub = CustomerSub;
            this.BookName = BookName;
            this.Author = Author;
            this.ISBN = ISBN;
            this.BookTypeId = BookTypeId;
            this.ConditionId = ConditionId;
            this.GenreId = GenreId;
            this.PublisherId = PublisherId;
            this.BookPrice = BookPrice;
        }

        public string CustomerSub { get; }
        public string BookName { get; }
        public string Author { get; }
        public string ISBN { get; }
        public int BookTypeId { get; }
        public int ConditionId { get; }
        public int GenreId { get; }
        public int PublisherId { get; }
        public decimal BookPrice { get; }
    }

    public class UpdateOfferStatusDto
    {
        public UpdateOfferStatusDto(
        int OfferId,
        OfferStatus Status)
        {
            this.OfferId = OfferId;
            this.Status = Status;
        }

        public int OfferId { get; }
        public OfferStatus Status { get; }
    }
}
