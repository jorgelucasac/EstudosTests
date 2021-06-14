using System.Linq;
using System.Threading;
using Features.Clientes;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace Features.Tests
{
    [Collection(nameof(ClienteAutoMockerCollection))]
    public class ClienteServiceFluentAssertionTests
    {
        readonly ClienteTestsAutoMockerFixture _clienteTestsAutoMockerFixture;

        private readonly ClienteService _clienteService;

        public ClienteServiceFluentAssertionTests(ClienteTestsAutoMockerFixture clienteTestsFixture)
        {
            _clienteTestsAutoMockerFixture = clienteTestsFixture;
            _clienteService = _clienteTestsAutoMockerFixture.ObterClienteService();
        }

        [Fact(DisplayName = "Adicionar Cliente com Sucesso")]
        [Trait("Categoria", "Cliente Service Fluent Assertion Tests")]
        public void ClienteService_Adicionar_DeveExecutarComSucesso()
        {
            // Arrange
            var cliente = _clienteTestsAutoMockerFixture.GerarClienteValido();

            // Act
            _clienteService.Adicionar(cliente);

            // Asert
            //Assert.True(cliente.EhValido());


            // Assert
            cliente.EhValido().Should().BeTrue();
            //validando se os metodos foram chamados na ação "clienteService.Adicionar"
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Once);
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Cliente com Falha")]
        [Trait("Categoria", "Cliente Service Fluent Assertion Tests")]
        public void ClienteService_Adicionar_DeveFalharDevidoClienteInvalido()
        { // Arrange
            var cliente = _clienteTestsAutoMockerFixture.GerarClienteInvalido();

            // Act
            _clienteService.Adicionar(cliente);

            // Asert
            //Assert.False(cliente.EhValido());

            // Assert
            cliente.EhValido().Should().BeFalse("Possui inconsistências");
            cliente.ValidationResult.Errors.Should().HaveCountGreaterOrEqualTo(1);

            //validando se os metodos foram chamados na ação "clienteService.Adicionar"
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Never);
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Obter Clientes Ativos")]
        [Trait("Categoria", "Cliente Service Fluent Assertion Tests")]
        public void ClienteService_ObterTodosAtivos_DeveRetornarApenasClientesAtivos()
        {
            // Arrange
            // Setando o retorno do método
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IClienteRepository>().Setup(c => c.ObterTodos())
                .Returns(_clienteTestsAutoMockerFixture.ObterClientesVariados());


            // Act
            var clientes = _clienteService.ObterTodosAtivos().ToList();

            // Assert
            //Assert.True(clientes.Any());
            //Assert.False(clientes.Count(c => !c.Ativo) > 0);

            // Assert
            clientes.Should().HaveCountGreaterOrEqualTo(1).And.OnlyHaveUniqueItems();
            clientes.Should().NotContain(c => !c.Ativo);

            _clienteTestsAutoMockerFixture.Mocker.GetMock<IClienteRepository>().Verify(r => r.ObterTodos(), Times.Once);
           
        }
    }
}