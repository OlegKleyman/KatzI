using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AspNetCoreTestServer.Core;
using BoDi;
using Katz.Data.Contexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;

namespace Katz.Web.Website.Tests.Integration
{
    [Binding]
    public class Initializer : Steps
    {
        private static readonly object SyncLock;
        private readonly IObjectContainer _container;

        static Initializer() => SyncLock = new object();

        public Initializer(IObjectContainer container) => _container = container;

        [BeforeScenario(Order = int.MinValue)]
        public void CreateContainerBuilder()
        {
            lock (SyncLock)
            {
                SetupWebServer();
                SetupBlogDatabase();
                SetupSelenium();
            }
        }

        private void SetupSelenium()
        {
            // Hack due to selenium issue of resolving chromedriver.exe
            const string pathEnvironmentVariableName = "PATH";
            var path = Environment.GetEnvironmentVariable(pathEnvironmentVariableName);
            Environment.SetEnvironmentVariable(pathEnvironmentVariableName, path + ";.");
            /////////////////////////////////////////////////////////////

            var options = new ChromeOptions();
            options.AddArguments("headless");

            var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(".", "chromedriver.exe"), options);

            driver.Manage().Window.Maximize();

            _container.RegisterFactoryAs<IWebDriver>(container => driver);
        }

        private void SetupWebServer()
        {
            _container.RegisterTypeAs<PortResolver, IPortResolver>();
            _container.RegisterFactoryAs(container => (Func<IWebHostBuilder>) WebHost.CreateDefaultBuilder);
            _container.RegisterTypeAs<KestrelWebServer, IWebServer>();
        }

        private void SetupBlogDatabase()
        {
            var defaultBuilder = new SqlConnectionStringBuilder
            {
                IntegratedSecurity = true,
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                MultipleActiveResultSets = true,
                InitialCatalog = $"blog_{Guid.NewGuid():N}",
                Pooling = false,
                ConnectTimeout = 120
            };

            var defaultOptions = new DbContextOptionsBuilder<BookContext>()
                                 .UseSqlServer(defaultBuilder.ToString())
                                 .Options;
            _container.RegisterFactoryAs(container =>
                new BookContext(defaultOptions));
        }

        [AfterScenario(Order = int.MaxValue)]
        public async Task Cleanup()
        {
            BookContext context;

            lock (SyncLock)
            {
                context = _container.Resolve<BookContext>();
                ScenarioContext.Get<RunningState>().Dispose();
                _container.Resolve<IWebDriver>().Quit();
            }

            await context.Database.EnsureDeletedAsync();
        }
    }
}