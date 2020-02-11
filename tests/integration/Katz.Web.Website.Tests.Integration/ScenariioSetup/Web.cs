using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AspNetCoreTestServer.Core;
using Katz.Data.Contexts;
using Katz.Web.Website.Tests.Integration.Support;
using Microsoft.EntityFrameworkCore;
using Optional;
using TechTalk.SpecFlow;

namespace Katz.Web.Website.Tests.Integration.ScenariioSetup
{
    [Binding]
    public class Web : Steps
    {
        public Web(ScenarioContext scenarioContext, IWebServer server)
        {
            scenarioContext.Set(server);
        }

        [BeforeScenario("web", Order = (int) FeatureOrder.Web)]
        public async Task Initialize()
        {
            var server = ScenarioContext.Get<IWebServer>();
            var assembly = typeof(Startup).Assembly;
            var hasDatabase = ScenarioContext.TryGetValue<BookContext>(out var context);
            var state = await server.StartAsync<Startup>(assembly, Path.GetDirectoryName(assembly.Location).Some(),
                new Dictionary<string, string>
                {
                    ["connectionStrings:default"] =
                        hasDatabase ? context.Database.GetDbConnection().ConnectionString : string.Empty
                });

            ScenarioContext.Set(state);
        }
    }
}