using System;
using System.Linq;
using FluentAssertions;
using Katz.Data.Entities;
using Katz.Web.Website.Tests.Integration.Support;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Katz.Web.Website.Tests.Integration.FeatureSteps
{
    [Binding]
    public class ViewBook : Steps
    {
        private readonly IWebDriver _driver;

        public ViewBook(IWebDriver driver) => _driver = driver;

        [Then(@"I will see its details")]
        public void ThenIWillSeeItsDetails()
        {
            var book = ScenarioContext.Get<Book>();
            var webBook = new
            {
                Title = _driver.FindElement(By.CssSelector($".book tr td:nth-child({BookDisplayPosition.Title:D})"))
                               .Text,
                Author = _driver.FindElement(By.CssSelector($".book tr td:nth-child({BookDisplayPosition.Author:D})"))
                                .Text,
                Description = _driver
                              .FindElement(
                                  By.CssSelector($".book tr td:nth-child({BookDisplayPosition.Description:D})"))
                              .Text,
                Series = _driver.FindElement(By.CssSelector($".book tr td:nth-child({BookDisplayPosition.Series:D})"))
                                .Text,
                Rating = int.Parse(_driver
                                   .FindElement(
                                       By.CssSelector($".book tr td:nth-child({BookDisplayPosition.Rating:D})"))
                                   .Text.Substring(0, 1)),
                CoverArt = new Uri(_driver.FindElement(By.CssSelector("header img")).GetAttribute("src"))
            };

            webBook.Should()
                   .BeEquivalentTo(book,
                       options => options.Excluding(info => info.Image)
                                         .Excluding(info => info.ImageMimeType)
                                         .Excluding(info => info.Id));
            webBook.CoverArt.Should().Be($"data:image/png;base64,{Convert.ToBase64String(book.Image)}");
        }

        [Then(@"I will see the related books")]
        public void ThenIWillSeeTheRelatedBooks(Table table)
        {
            var books = table.Rows.Select(row => row["Title"]).ToArray();
            var webBooks = _driver.FindElements(By.CssSelector(".related a"))
                                  .Select(element => element.Text)
                                  .ToArray();

            webBooks.Should().BeEquivalentTo(books);
        }
    }
}