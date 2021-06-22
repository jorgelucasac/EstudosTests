using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;
using Xunit;

namespace NerdStore.WebApp.Tests
{
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class UsuarioTests
    {
        private readonly IntegrationTestsFixture<StartupWebTest> _testsFixture;

        public UsuarioTests(IntegrationTestsFixture<StartupWebTest> testsFixture)
        {
            _testsFixture = testsFixture;
        }


        [Fact(DisplayName = "Realizar cadastro com sucesso")]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async Task Usuario_RealizarCadastro_DeveExecutarComSucesso()
        {
            // Arrange
            var respostaInicial = await _testsFixture.Client.GetAsync("/Identity/Account/Register");
            respostaInicial.EnsureSuccessStatusCode();

            
            var antiForgeryToken = _testsFixture.ObterAntiForgeryToken(await respostaInicial.Content.ReadAsStringAsync());
            _testsFixture.GerarUserSenha();

            var formData = new Dictionary<string, string>()
            {
                { _testsFixture.AntiForgeryFieldName, antiForgeryToken },
                {"Input.Email",_testsFixture.UsuarioEmail},
                {"Input.Password",_testsFixture.UsuarioSenha},
                {"Input.ConfirmPassword",_testsFixture.UsuarioSenha}
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Register")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            // Act
            var postResponse = await _testsFixture.Client.SendAsync(postRequest);

            var responseString = await postResponse.Content.ReadAsStringAsync();

            postResponse.EnsureSuccessStatusCode();
            Assert.Contains($"Hello {_testsFixture.UsuarioEmail}!", responseString);
        }
    }
}