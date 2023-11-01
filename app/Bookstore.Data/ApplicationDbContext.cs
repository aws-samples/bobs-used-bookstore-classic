using System.ComponentModel.DataAnnotations.Schema;
using Bookstore.Domain.Addresses;
using Bookstore.Domain.Books;
using Bookstore.Domain.Carts;
using Bookstore.Domain.Customers;
using Bookstore.Domain.Offers;
using Bookstore.Domain.Orders;
using Bookstore.Domain.ReferenceData;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Bookstore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(string connectionString) : base(connectionString) { }

        public DbSet<Address> Address { get; set; }

        public DbSet<Book> Book { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<ShoppingCart> ShoppingCart { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<Offer> Offer { get; set; }

        public DbSet<ReferenceDataItem> ReferenceData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Update to remove the pluralization to match the modern version
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();


            modelBuilder.Entity<Customer>().Property(x => x.Sub).HasColumnType("nvarchar").HasMaxLength(450);
            modelBuilder.Entity<Customer>().HasIndex(x => x.Sub).IsUnique();

            modelBuilder.Entity<Book>().HasRequired(x => x.Publisher).WithMany().HasForeignKey(x => x.PublisherId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Book>().HasRequired(x => x.BookType).WithMany().HasForeignKey(x => x.BookTypeId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Book>().HasRequired(x => x.Genre).WithMany().HasForeignKey(x => x.GenreId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Book>().HasRequired(x => x.Condition).WithMany().HasForeignKey(x => x.ConditionId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Offer>().HasRequired(x => x.Publisher).WithMany().HasForeignKey(x => x.PublisherId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Offer>().HasRequired(x => x.BookType).WithMany().HasForeignKey(x => x.BookTypeId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Offer>().HasRequired(x => x.Genre).WithMany().HasForeignKey(x => x.GenreId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Offer>().HasRequired(x => x.Condition).WithMany().HasForeignKey(x => x.ConditionId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>().HasRequired(x => x.Customer).WithMany().WillCascadeOnDelete(false);

            // Update the Refernce Data Table to Match the modern version
            modelBuilder.Entity<ReferenceDataItem>().ToTable("ReferenceData");

            modelBuilder.Entity<ShoppingCartItem>().HasKey(x => new { x.Id, x.ShoppingCartId });
            modelBuilder.Entity<ShoppingCartItem>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Database.SetInitializer(new BookstoreDbInitializer());
        }
    }
}