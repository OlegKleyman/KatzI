using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Katz.Core;
using Katz.Web.Website.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Katz.Web.Website.Tests.Unit
{
    public class BookControllerTests
    {
        private static BooksController GetBooksController(IBookService service) => new BooksController(service);

        [Fact]
        public async Task GetReturns404WhenBookIsNotFound()
        {
            var service = Substitute.For<IBookService>();
            service.GetAsync(Arg.Any<int>())
                   .Returns(new Book(default, new BookDetail(string.Empty, string.Empty, string.Empty),
                       new BookInformation(default, string.Empty, new Image(string.Empty, Array.Empty<byte>()))));
            service.GetAsync(1).Returns((Book) null);
            var controller = GetBooksController(service);
            var result = await controller.Get(1);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetReturnsBookView()
        {
            var service = Substitute.For<IBookService>();
            service.GetAsync(default)
                   .ReturnsForAnyArgs(new Book(default, new BookDetail(string.Empty, string.Empty, string.Empty),
                       new BookInformation(default, string.Empty, new Image(string.Empty, Array.Empty<byte>()))));
            var controller = GetBooksController(service);
            var result = await controller.Get(default);
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Book");
        }

        [Fact]
        public async Task GetReturnsBookViewWithModelFromService()
        {
            var service = Substitute.For<IBookService>();
            var bookDetails = new BookDetail("title", "Author", "series");
            var image = new Image(nameof(Image.MimeType), new byte[] { 1 });
            var bookInfo = new BookInformation(Rating.FiveStars, "Description", image);
            var book = new Book(1, bookDetails, bookInfo);
            service.GetAsync(1).Returns(book);
            var books = new[]
            {
                new Book(2, new BookDetail("title2", book.Detail.Author, string.Empty),
                    new BookInformation(Rating.FiveStars, "description2", image)),
                new Book(2, new BookDetail("title3", "author2", book.Detail.Series),
                    new BookInformation(Rating.FiveStars, "description3", image))
            };

            service.GetRelatedBooksByAuthorOrSeriesAsync(book).Returns(books);

            var controller = GetBooksController(service);
            var result = await controller.Get(1);
            result.Should()
                  .BeOfType<ViewResult>()
                  .Which.Model.Should()
                  .BeEquivalentTo(new
                  {
                      book.Id,
                      book.Detail.Author,
                      book.Detail.Title,
                      book.Info.Description,
                      Image = new
                      {
                          book.Info.Image.MimeType,
                          Base64Value = Convert.ToBase64String(book.Info.Image.Value)
                      },
                      book.Info.Rating,
                      book.Detail.Series,
                      RelatedBooks = books.Select(b => new
                      {
                          b.Id,
                          b.Detail.Author,
                          b.Detail.Title,
                          b.Info.Description,
                          Image = new
                          {
                              b.Info.Image.MimeType,
                              Base64Value = Convert.ToBase64String(b.Info.Image.Value)
                          },
                          b.Info.Rating,
                          b.Detail.Series
                      })
                  });
        }
    }
}