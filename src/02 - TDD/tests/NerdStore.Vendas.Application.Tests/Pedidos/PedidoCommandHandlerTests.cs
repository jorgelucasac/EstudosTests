using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Moq.AutoMock;
using NerdStore.Vendas.Application.Commands;
using NerdStore.Vendas.Domain;
using Xunit;

namespace NerdStore.Vendas.Application.Tests.Pedidos
{
    public class PedidoCommandHandlerTests
    {
        [Fact(DisplayName = "Adicionar Item Novo Pedido com Sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_NovoPedido_DeveExecutarComSucesso()
        {
            // Arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(Guid.NewGuid(),
                Guid.NewGuid(), "Produto Teste", 2, 100);

            var moker = new AutoMocker();
            var pedidoHandler = moker.CreateInstance<PedidoCommandHandler>();

            // Act
            var result = await pedidoHandler.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            Assert.True(result);
            moker.GetMock<IPedidoRepository>().Verify(r => r.Adicionar(It.IsAny<Pedido>()), Times.Once);
            //moker.GetMock<IPedidoRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
            moker.GetMock<IMediator>().Verify(r => r.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }
    }

}