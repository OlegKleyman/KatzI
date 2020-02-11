using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Katz.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.NSubstitute;
using NSubstitute;
using Xunit;
using BookEntity = Katz.Data.Entities.Book;

namespace Katz.Core.Tests.Unit
{
    public class BookServiceTests
    {
        private BookService GetBookService(BookContext context) => new BookService(context);

        [Fact]
        public async Task AddAsyncAddsBookToDataStoreAndReturnsId()
        {
            var context = Substitute.For<BookContext>(new DbContextOptions<BookContext>());
            context.Books = Enumerable.Empty<BookEntity>().AsQueryable().BuildMockDbSet();

            var service = GetBookService(context);
            var arguments = new AddArguments(new BookDetail("title", "author", "series"),
                new BookInformation(Rating.FourStars, "description", new Image("type", new byte[] { 1 })));
            context.Books.AddAsync(Arg.Any<BookEntity>())
                   .ReturnsForAnyArgs((EntityEntry<BookEntity>) default)
                   .AndDoes(info => info.Arg<BookEntity>().Id = 1);
            var result = await service.AddAsync(arguments);
            await context.Books.Received()
                         .AddAsync(Arg.Is<BookEntity>(book =>
                             book.Series == arguments.Detail.Series && book.Title == arguments.Detail.Title &&
                             book.Author == arguments.Detail.Author && book.Rating == (int) arguments.Info.Rating &&
                             book.Description == arguments.Info.Description &&
                             book.Image == arguments.Info.Image.Value &&
                             book.ImageMimeType == arguments.Info.Image.MimeType));
            await context.Received().SaveChangesAsync();
            result.Should().Be(1);
        }

        [Fact]
        public async Task DeleteAsyncReturnsFalseWhenDeleteWasNotSuccessful()
        {
            var context = Substitute.For<BookContext>(new DbContextOptions<BookContext>());
            context.Books = Enumerable.Empty<BookEntity>().AsQueryable().BuildMockDbSet();
            context.Books.FindAsync(1).ReturnsForAnyArgs((BookEntity) null);

            var service = GetBookService(context);
            var result = await service.DeleteAsync(1);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsyncReturnsTrueWhenDeleteWasSuccessful()
        {
            var context = Substitute.For<BookContext>(new DbContextOptions<BookContext>());
            context.Books = Enumerable.Empty<BookEntity>().AsQueryable().BuildMockDbSet();
            context.Books.FindAsync(1)
                   .ReturnsForAnyArgs(new BookEntity
                   {
                       Id = 1
                   });

            var service = GetBookService(context);
            var result = await service.DeleteAsync(1);
            context.Books.Received().Remove(Arg.Is<BookEntity>(book => book.Id == 1));
            await context.Received().SaveChangesAsync();
            result.Should().BeTrue();
        }

        [Fact]
        public async Task EditAsyncReturnsTrueWhenDeleteWasSuccessful()
        {
            var context = Substitute.For<BookContext>(new DbContextOptions<BookContext>());
            context.Books = Enumerable.Empty<BookEntity>().AsQueryable().BuildMockDbSet();
            var entity = new BookEntity();
            context.Books.FindAsync(1).ReturnsForAnyArgs(entity);

            var service = GetBookService(context);
            var arguments = new UpdateArguments
            {
                Description = nameof(UpdateArguments.Description),
                Author = nameof(UpdateArguments.Description),
                Series = nameof(UpdateArguments.Description),
                Title = nameof(UpdateArguments.Description),
                Rating = Rating.FiveStars,
                Image = new Image("type", Array.Empty<byte>())
            };

            await service.EditAsync(1, arguments);
            entity.Should()
                  .BeEquivalentTo(new
                  {
                      arguments.Description,
                      arguments.Author,
                      arguments.Series,
                      arguments.Title,
                      Rating = (int) arguments.Rating,
                      ImageMimeType = arguments.Image.MimeType,
                      Image = arguments.Image.Value
                  });

            await context.Received().SaveChangesAsync();
        }

        [Fact]
        public async Task FindAsyncReturnsAnArrayOfMatchedBooks()
        {
            var books = new[]
            {
                new Book(1,
                    new BookDetail("title", "author", "series"),
                    new BookInformation(Rating.FourStars, "description", new Image(default, default))),
                new Book(2,
                    new BookDetail("title", "author", "series"),
                    new BookInformation(Rating.FourStars, "description", new Image(default, default)))
            };

            var context = new BookContext(new DbContextOptions<BookContext>());
            context.Books = Enumerable.Range(1, 2)
                                      .Select(i => new BookEntity
                                      {
                                          Author = "author",
                                          Title = "title",
                                          Rating = 4,
                                          Description = "description",
                                          Id = i,
                                          Series = "series",
                                          Image = default,
                                          ImageMimeType = default
                                      })
                                      .AsQueryable()
                                      .BuildMockDbSet();
            var t = await context.Books.ToArrayAsync();
            var service = GetBookService(context);
            var results = await service.FindAsync(new SearchArguments
            {
                Author = "author",
                Rating = Rating.FourStars,
                Series = "series",
                Title = "title"
            });

            results.Should().BeEquivalentTo(books);
        }

        [Fact]
        public async Task GetAsyncReturnsBlogWhenIdIsFound()
        {
            var context = new BookContext(new DbContextOptions<BookContext>());
            var entity = new BookEntity
            {
                Author = nameof(BookEntity.Author),
                Title = nameof(BookEntity.Title),
                Rating = 5,
                ImageMimeType = nameof(BookEntity.ImageMimeType),
                Image = new byte[] { 1 },
                Description = nameof(BookEntity.Description),
                Id = 1
            };

            context.Books = Enumerable.Empty<BookEntity>().AsQueryable().BuildMockDbSet();
            context.Books.FindAsync(1).ReturnsForAnyArgs(entity);
            var service = GetBookService(context);
            var result = await service.GetAsync(1);
            result.Should()
                  .BeEquivalentTo(new
                  {
                      entity.Id,
                      Detail = new
                      {
                          entity.Title,
                          entity.Author
                      },
                      Info = new
                      {
                          entity.Rating,
                          entity.Description,
                          Image = new
                          {
                              MimeType = entity.ImageMimeType,
                              Value = entity.Image
                          }
                      }
                  });
        }

        [Fact]
        public async Task GetAsyncReturnsNullWhenBookIsNotFound()
        {
            var context = new BookContext(new DbContextOptions<BookContext>());
            context.Books = Enumerable.Empty<BookEntity>().AsQueryable().BuildMockDbSet();
            context.Books.FindAsync(Arg.Any<int>()).Returns(new BookEntity());
            context.Books.FindAsync(1).Returns((BookEntity) null);
            var service = GetBookService(context);
            var result = await service.GetAsync(1);
            result.Should().Be(null);
        }

        [Fact]
        public async Task GetRelatedBooksByAuthorOrSeriesAsyncReturnsAnArrayOfRelatedBooks()
        {
            var books = new[]
            {
                new Book(2,
                    new BookDetail("title", "author", "series"),
                    new BookInformation(Rating.FourStars, "description", new Image(default, default)))
            };

            var context = new BookContext(new DbContextOptions<BookContext>());
            context.Books = Enumerable.Range(1, 2)
                                      .Select(i => new BookEntity
                                      {
                                          Author = "author",
                                          Title = "title",
                                          Rating = 4,
                                          Description = "description",
                                          Id = i,
                                          Series = "series",
                                          Image = default,
                                          ImageMimeType = default
                                      })
                                      .AsQueryable()
                                      .BuildMockDbSet();
            var t = await context.Books.ToArrayAsync();
            var service = GetBookService(context);
            var results = await service.GetRelatedBooksByAuthorOrSeriesAsync(new Book(1,
                new BookDetail("title", "author", "series"),
                new BookInformation(Rating.FourStars, "description", new Image(default, default))));

            results.Should().BeEquivalentTo(books);
        }
    }
}