using System;
using System.Linq;
using Xunit;
namespace NerdStore.Vendas.Domain.Tests
{
    public class PedidoTests
    {
        [Fact(DisplayName = "Adicionar Item Novo Pedido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_NovoPedido_DeveAtualizarValor()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);

            // Act
            pedido.AdicionarItem(pedidoItem);

            // Assert
            Assert.Equal(200, pedido.ValorTotal);
        }


        [Fact(DisplayName = "Adicionar Item Pedido Existente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_ItemExistente_DeveIncrimentarQuantidadeSomarValor()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 2, 100);
            pedido.AdicionarItem(pedidoItem);

            var pedidoItem2 = new PedidoItem(produtoId, "Produto Teste", 1, 100);

            // Act
            pedido.AdicionarItem(pedidoItem2);

            // Assert
            Assert.Equal(300, pedido.ValorTotal);
            Assert.Equal(1, pedido.PedidoItens.Count);
            Assert.Equal(3, pedido.PedidoItens.First(p => p.ProdutoId == produtoId).Quantidade);
        }

        [Fact(DisplayName = "Adicionar Item Pedido acima do permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_UnidadesItemAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", Pedido.MaxUnidadesItems + 1, 100);

            // Act & Assert 
            Assert.Throws<DomainException>(() => pedido.AdicionarItem(pedidoItem));
        }

        [Fact(DisplayName = "Adicionar Item Existente Soma Unidades acima do permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_ItemExistenteSomaUnidadesAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 1, 100);
            var pedidoItem2 = new PedidoItem(produtoId, "Produto Teste", Pedido.MaxUnidadesItems, 100);
            pedido.AdicionarItem(pedidoItem);
            // Act & Assert 
            Assert.Throws<DomainException>(() => pedido.AdicionarItem(pedidoItem2));
        }

        [Fact(DisplayName = "Atualizar Item Pedido Inexistente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemNaoExisteNaLista_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 1, 100);

            // Act & Assert 
            Assert.Throws<DomainException>(() => pedido.AtualizarItem(pedidoItem));
        }

        [Fact(DisplayName = "Atualizar Item Pedido Valido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemValido_DeveAtualizarQuantidade()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 1, 100);
            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", 5, 100);
            pedido.AdicionarItem(pedidoItem);
            var novaQuantidade = pedidoItemAtualizado.Quantidade;
            // Act
            pedido.AtualizarItem(pedidoItemAtualizado);

            // Assert 
            Assert.Equal(novaQuantidade, pedido.PedidoItens.First(p => p.ProdutoId == produtoId).Quantidade);
        }

        [Fact(DisplayName = "Atualizar Item Pedido Validar Total")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_PedidoComProdutosDiferentes_DeveAtualizarValorTotal()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItemExistente1 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            var pedidoItemExistente2 = new PedidoItem(produtoId, "Produto Teste", 3, 15);
            pedido.AdicionarItem(pedidoItemExistente1);
            pedido.AdicionarItem(pedidoItemExistente2);

            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", 5, 15);
            var totalPedido = pedidoItemExistente1.Quantidade * pedidoItemExistente1.ValorUnitario +
                              pedidoItemAtualizado.Quantidade * pedidoItemAtualizado.ValorUnitario;

            // Act
            pedido.AtualizarItem(pedidoItemAtualizado);

            // Assert
            Assert.Equal(totalPedido, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Atualizar Item Pedido Quantidade acima do permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemUnidadesAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 3, 15);
            pedido.AdicionarItem(pedidoItem);

            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", Pedido.MaxUnidadesItems + 1, 15);

            // Act & Assert
            Assert.Throws<DomainException>(() => pedido.AtualizarItem(pedidoItemAtualizado));
        }

        [Fact(DisplayName = "Remover Item Pedido Inexistente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void RemoverItemPedido_ItemInexistenteNaLista_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 3, 15);

            // Act & Assert
            Assert.Throws<DomainException>(() => pedido.RemoverItem(pedidoItem));
        }

        [Fact(DisplayName = "Remover Item Pedido Existente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void RemoverItemPedido_ItemExistenteNaLista_DeveAtualizarValor()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            var pedidoItem2 = new PedidoItem(produtoId, "Produto Teste", 3, 15);
            pedido.AdicionarItem(pedidoItem);
            pedido.AdicionarItem(pedidoItem2);


            var totalPedido = pedidoItem2.Quantidade * pedidoItem2.ValorUnitario;

            // Act
            pedido.RemoverItem(pedidoItem);

            // Assert
            Assert.Equal(totalPedido, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Aplicar Voucher Válido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void Pedido_AplicarVoucherValido_DeveRetornarSemErros()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var voucher = new Voucher("PROMO-15-REAIS", 15, null, 1,
                DateTime.Now.AddDays(15), true, false, TipoDescontoVoucher.Valor);
            // Act
            var result = pedido.AplicarVoucher(voucher);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Aplicar Voucher Inválido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void Pedido_AplicarVoucherInvalido_DeveRetornarComErros()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var voucher = new Voucher("", null, null, 0,
                DateTime.Now.AddDays(-1), false, true, TipoDescontoVoucher.Valor);
            // Act
            var result = pedido.AplicarVoucher(voucher);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "Aplicar voucher Tipo Valor")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_VoucherTipoValorDesconto_DeveDescontarDoValorTotal()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            var pedidoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            var pedidoItem2 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 3, 20);
            pedido.AdicionarItem(pedidoItem1);
            pedido.AdicionarItem(pedidoItem2);

            var voucher = new Voucher("PROMO-15-REAIS", 20, null, 1,
                DateTime.Now.AddDays(15), true, false, TipoDescontoVoucher.Valor);

            var valorComDesconto = pedido.ValorTotal - voucher.ValorDesconto;

            // Act
            pedido.AplicarVoucher(voucher);

            // Assert
            Assert.Equal(valorComDesconto, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Aplicar Voucher Tipo Percentual")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_VoucherTipoPercentualDesconto_DeveDescontarDoValorTotal()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            var pedidoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            var pedidoItem2 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 5, 20);
            pedido.AdicionarItem(pedidoItem1);
            pedido.AdicionarItem(pedidoItem2);

            var voucher = new Voucher("PROMO-15-REAIS", null, 50, 1,
                DateTime.Now.AddDays(15), true, false, TipoDescontoVoucher.Porcentagem);

            var valorComDesconto = pedido.ValorTotal - (voucher.PercentualDesconto / 100 * pedido.ValorTotal);

            // Act
            pedido.AplicarVoucher(voucher);

            // Assert
            Assert.Equal(valorComDesconto, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Aplicar Voucher Desconto Excede Valor Total")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_DescontoExcedeValorTotalPedido_PedidoDeveTerValorZero()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 5, 100);
            pedido.AdicionarItem(pedidoItem);

            var voucher = new Voucher("PROMO-15-REAIS", 1000, null, 1,
                DateTime.Now.AddDays(15), true, false, TipoDescontoVoucher.Valor);

            // Act
            pedido.AplicarVoucher(voucher);

            // Assert
            Assert.Equal(0, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Aplicar Voucher Recalcular Desconto na Modificação do Pedido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_ModificarItensPedido_DeveCalcularDescontoValorTotal()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            pedido.AdicionarItem(pedidoItem1);
            
            var voucher = new Voucher("PROMO-15-REAIS", 100, null, 1,
                DateTime.Now.AddDays(15), true, false, TipoDescontoVoucher.Valor);

            pedido.AplicarVoucher(voucher);

            var pedidoItem2 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 3, 100);
            
            
            // Act
            pedido.AdicionarItem(pedidoItem2);

            // Assert
             var totalEsperado = pedido.PedidoItens.Sum(i => i.Quantidade * i.ValorUnitario) - voucher.ValorDesconto;
            Assert.Equal(totalEsperado, pedido.ValorTotal);

        }
    }
}