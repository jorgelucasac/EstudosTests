using System;
using System.Linq;
using Xunit;
namespace NerdStore.Vendas.Domain.Tests
{
    public class VoucherTests
    {
        [Fact(DisplayName = "Validar Voucher Tipo Valor Válido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherTipoValor_DeveEstarValido()
        {
            // Arrange
            var voucher = new Voucher("PROMO-15-REAIS", 15, null, 1,
                DateTime.Now.AddDays(15), true, false, TipoDescontoVoucher.Valor);
            // Act
            var result = voucher.ValidarSeAplicavel();

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Validar Voucher Tipo Valor Inválido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherTipoValor_DeveEstarInvalido()
        {
            // Arrange
            var voucher = new Voucher("", null, null, 0,
                DateTime.Now.AddDays(-1), false, true, TipoDescontoVoucher.Valor);
            // Act
            var result = voucher.ValidarSeAplicavel();
            var erros = result.Errors.Select(c => c.ErrorMessage).ToList();

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(6, erros.Count);
            Assert.Contains(VoucherAplicavelValidation.AtivoErroMsg, erros);
            Assert.Contains(VoucherAplicavelValidation.CodigoErroMsg, erros);
            Assert.Contains(VoucherAplicavelValidation.DataValidadeErroMsg, erros);
            Assert.Contains(VoucherAplicavelValidation.QuantidadeErroMsg, erros);
            Assert.Contains(VoucherAplicavelValidation.UtilizadoErroMsg, erros);
            Assert.Contains(VoucherAplicavelValidation.ValorDescontoErroMsg, erros);
        }

        [Fact(DisplayName = "Validar Voucher Tipo Porcentagem Válido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherTipoPorcentagem_DeveEstarValido()
        {
            // Arrange
            var voucher = new Voucher("PROMO-15-REAIS", null, 50, 1,
                DateTime.Now.AddDays(15), true, false, TipoDescontoVoucher.Porcentagem);
            // Act
            var result = voucher.ValidarSeAplicavel();

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Validar Voucher Tipo Porcentagem Inválido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherTipoPorcentagem_DeveEstarInvalido()
        {
            // Arrange
            var voucher = new Voucher("", null, null, 0,
                DateTime.Now.AddDays(-1), false, true, TipoDescontoVoucher.Porcentagem);
            // Act
            var result = voucher.ValidarSeAplicavel();
            var erros = result.Errors.Select(c => c.ErrorMessage).ToList();
            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(6, erros.Count);
            Assert.Contains(VoucherAplicavelValidation.AtivoErroMsg, erros);
            Assert.Contains(VoucherAplicavelValidation.CodigoErroMsg, erros);
            Assert.Contains(VoucherAplicavelValidation.DataValidadeErroMsg, erros);
            Assert.Contains(VoucherAplicavelValidation.QuantidadeErroMsg, erros);
            Assert.Contains(VoucherAplicavelValidation.UtilizadoErroMsg, erros);
            Assert.Contains(VoucherAplicavelValidation.PercentualDescontoErroMsg, erros);
        }
    }
}