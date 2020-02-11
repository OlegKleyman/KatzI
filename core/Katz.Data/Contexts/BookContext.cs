using Katz.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Katz.Data.Contexts
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var bookEntity = modelBuilder.Entity<Book>();
            bookEntity.HasKey(book => book.Id);
            bookEntity.Property(book => book.Author).IsRequired();
            bookEntity.Property(book => book.Description).IsRequired();
            bookEntity.Property(book => book.Image).IsRequired();
            bookEntity.Property(book => book.ImageMimeType).IsRequired();
            bookEntity.Property(book => book.Title).IsRequired();
            base.OnModelCreating(modelBuilder);
        }
    }
}