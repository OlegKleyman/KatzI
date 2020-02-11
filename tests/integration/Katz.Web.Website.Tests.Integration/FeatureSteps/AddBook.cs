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
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Katz.Web.Website.Tests.Integration.FeatureSteps
{
    [Binding]
    public class AddBook : Steps
    {
        private readonly IWebDriver _driver;

        public AddBook(IWebDriver driver) => _driver = driver;

        [Given(@"I want to add a book")]
        public async Task GivenIWantToAddABook(Table table)
        {
            var book = table.CreateInstance<Book>();
            _driver.FindElement(By.Name("Author")).SendKeys(book.Author);
            _driver.FindElement(By.Name("Title")).SendKeys(book.Title);
            _driver.FindElement(By.Name("Description")).SendKeys(book.Description);
            var ratingInput = _driver.FindElement(By.Name("Rating"));
            var selectList = new SelectElement(ratingInput);
            selectList.SelectByValue(book.Rating.ToString());
            _driver.FindElement(By.Name("Series")).SendKeys(book.Series ?? string.Empty);
            var imagePath = Path.GetFullPath(@"Resources\test.png");
            _driver.FindElement(By.Name("Image")).SendKeys(imagePath);
            new FileExtensionContentTypeProvider().TryGetContentType(imagePath, out var contentType);
            book.ImageMimeType = contentType;
            book.Image = await File.ReadAllBytesAsync(imagePath);
            ScenarioContext.Set(book);
        }

        [Given(@"I am on the add page")]
        public void GivenIAmOnTheAddPage()
        {
            var state = ScenarioContext.Get<RunningState>();
            var searchUrl = new Uri(state.Endpoint, "books/add");
            _driver.Navigate().GoToUrl(searchUrl);
        }

        [When(@"I press the submit button")]
        public void WhenIPressTheSubmitButton()
        {
            _driver.FindElement(By.CssSelector("form button")).Click();
        }

        [Then(@"the book will be added to the database")]
        public void ThenTheBookWillBeAddedToTheDatabase()
        {
            var book = ScenarioContext.Get<Book>();
            var context = ScenarioContext.Get<BookContext>();
            context.Books.Single().Should().BeEquivalentTo(book, options => options.Excluding(info => info.Id));
        }
    }
}