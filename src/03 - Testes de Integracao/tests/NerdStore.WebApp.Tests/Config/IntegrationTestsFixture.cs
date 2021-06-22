using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using Bogus;
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

        public string UsuarioEmail;
        public string UsuarioSenha;

        public string AntiForgeryFieldName = "__RequestVerificationToken";

        public IntegrationTestsFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions{};


            _factory = new LojaAppFactory<TStartup>();
            Client = _factory.CreateClient(clientOptions);
           // Client.BaseAddress = new Uri("https://localhost:44396/");
        }


        public void GerarUserSenha()
        {
            var faker = new Faker("pt_BR");
            UsuarioEmail = faker.Internet.Email().ToLower();
            UsuarioSenha = faker.Internet.Password(8, false, "", "@1Ab_");
        }

        public string ObterAntiForgeryToken(string htmlBody)
        {
            var requestVerificationTokenMatch =
                Regex.Match(htmlBody, $@"\<input name=""{AntiForgeryFieldName}"" type=""hidden"" value=""([^""]+)"" \/\>");

            if (requestVerificationTokenMatch.Success)
            {
                return requestVerificationTokenMatch.Groups[1].Captures[0].Value;
            }

            throw new ArgumentException($"Anti forgery token '{AntiForgeryFieldName}' não encontrado no HTML", nameof(htmlBody));
        }

        public void Dispose()
        {
            Client?.Dispose();
            _factory?.Dispose();
        }
    }
}