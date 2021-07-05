using NerdStore.BDD.Tests.Config;
using TechTalk.SpecFlow;
using Xunit;

namespace NerdStore.BDD.Tests.Usuarios
{
    [Binding]
    [CollectionDefinition(nameof(AutomacaoWebFixtureCollection))]
    public class CommomSteps
    {
        private readonly CadastroDeUsuarioTela _cadastroDeUsuarioTela;
        private readonly AutomacaoWebTestsFixture _testsFixture;

        public CommomSteps(AutomacaoWebTestsFixture testsFixture)
        {
            _testsFixture = testsFixture;
            _cadastroDeUsuarioTela = new CadastroDeUsuarioTela(_testsFixture.BrowserHelper);
        }


        [Given(@"Que o visitante está acessando o site da loja")]
        public void DadoQueOVisitanteEstaAcessandoOSiteDaLoja()
        {
            // Act
            _cadastroDeUsuarioTela.AcessarSiteLoja();

            // Assert
            Assert.Contains(_testsFixture.Configuration.DomainUrl, _cadastroDeUsuarioTela.ObterUrl());

        }

        [Then(@"Ele será redirecionado para a vitrine")]
        public void EntaoEleSeraRedirecionadoParaAVitrine()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"Uma saudação com seu e-mail será exibida no menu superior")]
        public void EntaoUmaSaudacaoComSeuE_MailSeraExibidaNoMenuSuperior()
        {
            ScenarioContext.Current.Pending();
        }
    }
}