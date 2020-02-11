using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreTestServer.Core;
using Katz.Data.Contexts;
using Katz.Data.Entities;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Katz.Web.Website.Tests.Integration.FeatureSteps
{
    [Binding]
    public class Common : Steps
    {
        private readonly IWebDriver _driver;

        public Common(IWebDriver driver) => _driver = driver;

        [Given(@"There exists a book")]
        public async Task GivenThereExistsABook()
        {
            CreateDefaultBook();
            var book = ScenarioContext.Get<Book>();
            var context = ScenarioContext.Get<BookContext>();
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();
        }

        private void CreateDefaultBook()
        {
            var book = new Book
            {
                Title = nameof(Book.Title),
                Author = nameof(Book.Author),
                Description = nameof(Book.Description),
                Series = nameof(Book.Series),
                Rating = 5,
                ImageMimeType = "image/png",
                Image = Images.Test
            };

            ScenarioContext.Set(book);
        }

        [When(@"I view it")]
        public void WhenIViewIt()
        {
            var book = ScenarioContext.Get<Book>();
            var state = ScenarioContext.Get<RunningState>();
            var url = new Uri(state.Endpoint, $"/books/{book.Id}");
            _driver.Navigate().GoToUrl(url);
        }

        [Given(@"There are books")]
        public async Task GivenThereAreBooks(Table table)
        {
            var books = table.CreateSet<Book>().ToArray();
            var context = ScenarioContext.Get<BookContext>();
            foreach (var book in books)
            {
                book.ImageMimeType = "image/png";
                book.Image = Images.Test;
                await context.Books.AddAsync(book);
            }

            await context.SaveChangesAsync();
            ScenarioContext.Set(books);
        }

        [When(@"I view ""(.+)""")]
        public void WhenIView(string title)
        {
            var books = ScenarioContext.Get<Book[]>();
            var targetBook = books.Single(book => book.Title == title);
            var state = ScenarioContext.Get<RunningState>();
            var url = new Uri(state.Endpoint, $"/books/{targetBook.Id}");
            _driver.Navigate().GoToUrl(url);
        }
    }
}