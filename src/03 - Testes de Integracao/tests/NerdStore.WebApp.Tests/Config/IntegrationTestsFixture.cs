using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using NerdStore.WebApp.MVC;
using Xunit;

namespace NerdStore.WebApp.Tests.Config
{
    [CollectionDefinition(nameof(IntegrationWebTestsFixtureCollection))]
    public class IntegrationWebTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture<StartupWebTest>> { }


    [CollectionDefinition(nameof(IntegrationApiTestsFixtureCollection))]
    public class IntegrationApiTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture<StartupApiTest>> { }

    public class IntegrationTestsFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly LojaAppFactory<TStartup> _factory;
        public HttpClient Client;

        public IntegrationTestsFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions{};


            _factory = new LojaAppFactory<TStartup>();
            Client = _factory.CreateClient(clientOptions);
        }

        public void Dispose()
        {
            Client?.Dispose();
            _factory?.Dispose();
        }
    }
}