using System.Linq;
using FluentAssertions;
using Katz.Data.Contexts;
using Katz.Data.Entities;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Katz.Web.Website.Tests.Integration.FeatureSteps
{
    [Binding]
    public class DeleteBook : Steps
    {
        private readonly IWebDriver _driver;

        public DeleteBook(IWebDriver driver) => _driver = driver;

        [When(@"click the delete link")]
        public void WhenClickTheDeleteLink()
        {
            _driver.FindElement(By.CssSelector("form button")).Click();
        }

        [Then(@"it will be deleted")]
        public void ThenItWillBeDeleted()
        {
            var book = ScenarioContext.Get<Book>();
            var context = ScenarioContext.Get<BookContext>();
            context.Books.AsNoTracking().Where(b => b.Id == book.Id).Should().BeEmpty();
        }
    }
}