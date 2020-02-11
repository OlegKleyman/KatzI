using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreTestServer.Core;
using FluentAssertions;
using Katz.Data.Contexts;
using Katz.Data.Entities;
using Microsoft.AspNetCore.StaticFiles;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Katz.Web.Website.Tests.Integration.FeatureSteps
{
    [Binding]
    public class UpdateBook : Steps
    {
        private readonly IWebDriver _driver;

        public UpdateBook(IWebDriver driver) => _driver = driver;

        [Given(@"I am on the update page")]
        public void GivenIAmOnTheUpdatePage()
        {
            var state = ScenarioContext.Get<RunningState>();
            var book = ScenarioContext.Get<Book>();
            var searchUrl = new Uri(state.Endpoint, $"books/{book.Id}/edit");
            _driver.Navigate().GoToUrl(searchUrl);
        }

        [When(@"I update the book")]
        public async Task WhenIUpdateTheBook(Table table)
        {
            var existingBook = ScenarioContext.Get<Book>();
            var book = table.CreateInstance(() => existingBook);
            _driver.FindElement(By.Name("Author")).SendKeys(book.Author);
            _driver.FindElement(By.Name("Title")).SendKeys(book.Title);
            _driver.FindElement(By.Name("Description")).SendKeys(book.Description);
            var ratingInput = _driver.FindElement(By.Name("Rating"));
            ratingInput.Clear();
            ratingInput.SendKeys(book.Rating.ToString());
            _driver.FindElement(By.Name("Series")).SendKeys(book.Series ?? string.Empty);
            var imagePath = Path.GetFullPath(@"Resources\TestJpeg.jpg");
            _driver.FindElement(By.Name("Image")).SendKeys(imagePath);
            new FileExtensionContentTypeProvider().TryGetContentType(imagePath, out var contentType);
            book.ImageMimeType = contentType;
            book.Image = await File.ReadAllBytesAsync(imagePath);
        }

        [Then(@"the book will be updated in the database")]
        public void ThenTheBookWillBeUpdatedInTheDatabase()
        {
            var book = ScenarioContext.Get<Book>();
            var context = ScenarioContext.Get<BookContext>();

            context.Books.Single().Should().BeEquivalentTo(book);
        }
    }
}