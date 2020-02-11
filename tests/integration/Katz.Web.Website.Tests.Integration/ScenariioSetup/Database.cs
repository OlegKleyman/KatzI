using System.Threading.Tasks;
using Katz.Data.Contexts;
using Katz.Web.Website.Tests.Integration.Support;
using TechTalk.SpecFlow;

namespace Katz.Web.Website.Tests.Integration.ScenariioSetup
{
    [Binding]
    public class Database : Steps
    {
        public Database(ScenarioContext scenarioContext, BookContext context)
        {
            scenarioContext.Set(context);
        }

        [BeforeScenario("database", Order = (int) FeatureOrder.Database)]
        public async Task Initialize()
        {
            await ScenarioContext.Get<BookContext>().Database.EnsureCreatedAsync();
        }
    }
}