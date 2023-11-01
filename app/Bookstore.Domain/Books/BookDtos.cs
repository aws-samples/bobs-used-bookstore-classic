using System.IO;

namespace Bookstore.Domain.Books
{
    public class CreateBookDto
    {
        public CreateBookDto(
        string Name,
        string Author,
        int BookTypeId,
        int ConditionId,
        int GenreId,
        int PublisherId,
        int? Year,
        string ISBN,
        string Summary,
        decimal Price,
        int Quantity,
        Stream CoverImage,
        string CoverImageFileName)
        {
            this.Name = Name;
            this.Author = Author;
            this.BookTypeId = BookTypeId;
            this.ConditionId = ConditionId;
            this.GenreId = GenreId;
            this.PublisherId = PublisherId;
            this.Year = Year;
            this.ISBN = ISBN;
            this.Summary = Summary;
            this.Price = Price;
            this.Quantity = Quantity;
            this.CoverImage = CoverImage;
            this.CoverImageFileName = CoverImageFileName;
        }

        public string Name { get; }
        public string Author { get; }
        public int BookTypeId { get; }
        public int ConditionId { get; }
        public int GenreId { get; }
        public int PublisherId { get; }
        public int? Year { get; }
        public string ISBN { get; }
        public string Summary { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public Stream CoverImage { get; }
        public string CoverImageFileName { get; }
    }

    public class UpdateBookDto
    {
        public UpdateBookDto(
        int BookId,
        string Name,
        string Author,
        int BookTypeId,
        int ConditionId,
        int GenreId,
        int PublisherId,
        int? Year,
        string ISBN,
        string Summary,
        decimal Price,
        int Quantity,
        Stream CoverImage,
        string CoverImageFileName)
        {
            this.BookId = BookId;
            this.Name = Name;
            this.Author = Author;
            this.BookTypeId = BookTypeId;
            this.ConditionId = ConditionId;
            this.GenreId = GenreId;
            this.PublisherId = PublisherId;
            this.Year = Year;
            this.ISBN = ISBN;
            this.Summary = Summary;
            this.Price = Price;
            this.Quantity = Quantity;
            this.CoverImage = CoverImage;
            this.CoverImageFileName = CoverImageFileName;
        }

        public int BookId { get; }
        public string Name { get; }
        public string Author { get; }
        public int BookTypeId { get; }
        public int ConditionId { get; }
        public int GenreId { get; }
        public int PublisherId { get; }
        public int? Year { get; }
        public string ISBN { get; }
        public string Summary { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public Stream CoverImage { get; }
        public string CoverImageFileName { get; }
    }
}