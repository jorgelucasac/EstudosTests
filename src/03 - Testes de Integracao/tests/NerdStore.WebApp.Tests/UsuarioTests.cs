using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;

namespace NerdStore.WebApp.Tests
{
    public class UsuarioTests
    {
        private readonly IntegrationTestsFixture<StartupWebTest> _testsFixture;

        public UsuarioTests(IntegrationTestsFixture<StartupWebTest> testsFixture)
        {
            _testsFixture = testsFixture;
        }
    }
}