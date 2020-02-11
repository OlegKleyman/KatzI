using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreTestServer.Core;
using FluentAssertions;
using Katz.Core;
using Katz.Data.Contexts;
using Katz.Web.Website.Tests.Integration.Support;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using BookEntity = Katz.Data.Entities.Book;

namespace Katz.Web.Website.Tests.Integration.FeatureSteps
{
    [Binding]
    public class SearchBooks : Steps
    {
        private readonly IWebDriver _driver;

        public SearchBooks(IWebDriver driver) => _driver = driver;

        [Given(@"I am on the search page")]
        public void GivenIAmOnTheSearchPage()
        {
            var state = ScenarioContext.Get<RunningState>();
            var searchUrl = new Uri(state.Endpoint, "books/search");
            _driver.Navigate().GoToUrl(searchUrl);
        }

        [When(@"I search for")]
        public async Task WhenISearchFor(Table table)
        {
            var arguments = table.CreateInstance<SearchArguments>();
            var context = ScenarioContext.Get<BookContext>();

            var expected = await context.Books
                                        .Where(book => string.IsNullOrWhiteSpace(arguments.Author) ||
                                                       book.Author == arguments.Author)
                                        .Where(book =>
                                            string.IsNullOrWhiteSpace(arguments.Title) || book.Title == arguments.Title)
                                        .Where(book =>
                                            !arguments.Rating.HasValue || book.Rating == (int) arguments.Rating)
                                        .Where(book =>
                                            string.IsNullOrWhiteSpace(arguments.Series) ||
                                            book.Series == arguments.Series)
                                        .ToArrayAsync();
            ScenarioContext.Set(expected);
            _driver.FindElement(By.Name("Author")).SendKeys(arguments.Author ?? string.Empty);
            _driver.FindElement(By.Name("Title")).SendKeys(arguments.Title ?? string.Empty);
            if (arguments.Rating.HasValue)
            {
                var selectElement = new SelectElement(_driver.FindElement(By.Name("Rating")));
                selectElement.SelectByValue(arguments.Rating.Value.ToString("D"));
            }

            _driver.FindElement(By.Name("Series")).SendKeys(arguments.Series ?? string.Empty);
            _driver.FindElement(By.CssSelector("table button")).Click();
        }

        [Then(@"I will see the following results")]
        public void ThenIWillSeeTheFollowingResults(Table table)
        {
            var expected = ScenarioContext.Get<BookEntity[]>();

            var results = _driver.FindElements(By.CssSelector("table"))[1]
                                 .FindElements(By.CssSelector("tbody tr"))
                                 .Select(element => element.FindElements(By.CssSelector("td")))
                                 .Select(elements => new
                                 {
                                     Author = elements[(int) BookDisplayPosition.Author - 1].Text,
                                     Title = elements[(int) BookDisplayPosition.Title - 1].Text,
                                     Rating = int.Parse(elements[(int) BookDisplayPosition.Rating - 1].Text),
                                     Series = elements[(int) BookDisplayPosition.Series - 1].Text,
                                     Description = elements[(int) BookDisplayPosition.Description - 1].Text
                                 });

            results.Should()
                   .BeEquivalentTo(expected.AsEnumerable(),
                       options => options.Excluding(book => book.Id)
                                         .Excluding(book => book.Image)
                                         .Excluding(book => book.ImageMimeType));
        }
    }
}