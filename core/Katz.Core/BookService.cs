using System.Linq;
using System.Threading.Tasks;
using Katz.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using BookEntity = Katz.Data.Entities.Book;

namespace Katz.Core
{
    public class BookService : IBookService
    {
        private readonly BookContext _context;

        public BookService(BookContext context) => _context = context;

        public async Task<Book> GetAsync(int id)
        {
            var entity = await _context.Books.FindAsync(id);
            Book book = null;

            if (!(entity is null))
            {
                book = new Book(entity.Id, new BookDetail(entity.Title, entity.Author, entity.Series),
                    new BookInformation((Rating) entity.Rating, entity.Description,
                        new Image(entity.ImageMimeType, entity.Image)));
            }

            return book;
        }

        public async Task<Book[]> GetRelatedBooksByAuthorOrSeriesAsync(Book book)
        {
            var books = await _context.Books.Where(b => b.Id != book.Id &&
                                                        (!string.IsNullOrEmpty(b.Author) &&
                                                         b.Author == book.Detail.Author ||
                                                         !string.IsNullOrEmpty(b.Series) &&
                                                         b.Series == book.Detail.Series))
                                      .ToArrayAsync();
            return books.Select(b => new Book(b.Id, new BookDetail(b.Title, b.Author, b.Series),
                            new BookInformation((Rating) b.Rating, b.Description,
                                new Image(b.ImageMimeType, b.Image))))
                        .ToArray();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            var result = false;
            if (!(book is null))
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                result = true;
            }

            return result;
        }

        public async Task<Book[]> FindAsync(SearchArguments arguments)
        {
            var results = await _context.Books
                                        .Where(book =>
                                            string.IsNullOrWhiteSpace(arguments.Author) ||
                                            book.Author == arguments.Author)
                                        .Where(book =>
                                            string.IsNullOrWhiteSpace(arguments.Title) || book.Title == arguments.Title)
                                        .Where(book =>
                                            !arguments.Rating.HasValue || book.Rating == (int) arguments.Rating)
                                        .Where(book =>
                                            string.IsNullOrWhiteSpace(arguments.Series) ||
                                            book.Series == arguments.Series)
                                        .Select(book => new Book(book.Id,
                                            new BookDetail(book.Title, book.Author, book.Series),
                                            new BookInformation((Rating) book.Rating, book.Description,
                                                new Image(book.ImageMimeType, book.Image))))
                                        .ToArrayAsync();
            return results;
        }

        public async Task<int> AddAsync(AddArguments arguments)
        {
            var book = new BookEntity
            {
                Author = arguments.Detail.Author,
                Title = arguments.Detail.Title,
                Series = arguments.Detail.Series,
                Description = arguments.Info.Description,
                Rating = (int) arguments.Info.Rating,
                ImageMimeType = arguments.Info.Image.MimeType,
                Image = arguments.Info.Image.Value
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            return book.Id;
        }

        public async Task EditAsync(int id, UpdateArguments arguments)
        {
            var book = await _context.Books.FindAsync(id);
            book.Author = arguments.Author ?? book.Author;
            book.Description = arguments.Description ?? book.Description;
            book.Image = arguments.Image?.Value ?? book.Image;
            book.ImageMimeType = arguments.Image?.MimeType ?? book.ImageMimeType;
            book.Series = arguments.Series ?? book.Series;
            book.Title = arguments.Title ?? book.Title;
            if (arguments.Rating.HasValue)
            {
                book.Rating = (int) arguments.Rating.Value;
            }

            await _context.SaveChangesAsync();
        }
    }
}