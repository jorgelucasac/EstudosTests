using Features.Clientes;
using MediatR;
using Moq;
using Moq.AutoMock;
using System.Linq;
using System.Threading;
using Xunit;

namespace Features.Tests
{
    [Collection(nameof(ClienteBogusCollection))]
    public class ClienteServiceAutoMockerTests
    {
        private readonly ClienteTestsBogusFixture _clienteTestsBogus;

        public ClienteServiceAutoMockerTests(ClienteTestsBogusFixture clienteTestsFixture)
        {
            _clienteTestsBogus = clienteTestsFixture;
        }

        [Fact(DisplayName = "Adicionar Cliente com Sucesso")]
        [Trait("Categoria", "Cliente Service AutoMock Tests")]
        public void ClienteService_Adicionar_DeveExecutarComSucesso()
        {
            // Arrange
            var cliente = _clienteTestsBogus.GerarClienteValido();
            var moker = new AutoMocker();

            //deve-se usar a classe concreta
            var clienteService = moker.CreateInstance<ClienteService>();

            // Act
            clienteService.Adicionar(cliente);

            // Asert
            Assert.True(cliente.EhValido());

            //validando se os metodos foram chamados na ação "clienteService.Adicionar"
            moker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Once);
            moker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Cliente com Falha")]
        [Trait("Categoria", "Cliente Service AutoMock Tests")]
        public void ClienteService_Adicionar_DeveFalharDevidoClienteInvalido()
        {
            // Arrange
            var cliente = _clienteTestsBogus.GerarClienteInvalido();
            var moker = new AutoMocker();

            //deve-se usar a classe concreta
            var clienteService = moker.CreateInstance<ClienteService>();

            // Act
            clienteService.Adicionar(cliente);

            // Asert
            Assert.False(cliente.EhValido());

            //validando se os metodos foram chamados na ação "clienteService.Adicionar"
            moker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Never);
            moker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Obter Clientes Ativos")]
        [Trait("Categoria", "Cliente Service AutoMock Tests")]
        public void ClienteService_ObterTodosAtivos_DeveRetornarApenasClientesAtivos()
        {
            // Arrange
            var moker = new AutoMocker();

            //deve-se usar a classe concreta
            var clienteService = moker.CreateInstance<ClienteService>();

            // Setando o retorno do método
            moker.GetMock<IClienteRepository>().Setup(c => c.ObterTodos())
                .Returns(_clienteTestsBogus.ObterClientesVariados());


            // Act
            var clientes = clienteService.ObterTodosAtivos().ToList();

            // Assert
            moker.GetMock<IClienteRepository>().Verify(r => r.ObterTodos(), Times.Once);
            Assert.True(clientes.Any());
            Assert.False(clientes.Count(c => !c.Ativo) > 0);
        }
    }
}