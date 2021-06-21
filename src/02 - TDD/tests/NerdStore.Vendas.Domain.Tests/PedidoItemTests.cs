using System;
using NerdStore.Core.DomainObjects;
using Xunit;

namespace NerdStore.Vendas.Domain.Tests
{
    public class PedidoItemTests
    {
        [Fact(DisplayName = "Novo Item Pedido abaixo do permitido")]
        [Trait("Categoria", "Vendas - Pedido Item")]
        public void AdicionarItemPedido_UnidadesItemAbaixoDoPermitido_DeveRetornarException()
        {
            //Arrange & Act & Assert 
            Assert.Throws<DomainException>(() =>
                new PedidoItem(Guid.NewGuid(), "Produto Teste", Pedido.MinUnidadesItems - 1, 100));
        }
    }
}