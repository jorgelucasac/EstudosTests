using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Vendas.Domain
{
    public class Pedido : Entity, IAggregateRoot
    {
        public const int MaxUnidadesItems = 15;
        public const int MinUnidadesItems = 1;
        public int Codigo { get; private set; }
        public Guid ClienteId { get; private set; }
        public Guid? VoucherId { get; private set; }
        public decimal ValorTotal { get; private set; }
        public PedidoStatus PedidoStatus { get; private set; }
        public decimal Desconto { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public bool VoucherUtilizado { get; private set; }
        public Voucher Voucher { get; private set; }

        private readonly List<PedidoItem> _pedidoItens;
        public IReadOnlyCollection<PedidoItem> PedidoItens => _pedidoItens;

        protected Pedido()
        {
            _pedidoItens = new List<PedidoItem>();
        }

        private void CalcularValorPedido()
        {
            ValorTotal = _pedidoItens.Sum(i => i.CalcularValorTotal());
            CalcularValorDesconto();
        }

        public bool PedidoItemExiste(PedidoItem pedidoItem)
        {
            return _pedidoItens.Any(p => p.ProdutoId == pedidoItem.ProdutoId);
        }

        private void ValidarPedidoItemInexistente(PedidoItem pedidoItem)
        {
            if (!PedidoItemExiste(pedidoItem))
                throw new DomainException($"O item não existe no pedido");
        }

        public PedidoItem ObterPedidoItemExistente(Guid produtoId)
        {
            return _pedidoItens.FirstOrDefault(a => a.ProdutoId == produtoId);
        }

        private void ValidarQuantidadeMaximaItem(PedidoItem pedidoItem)
        {
            var quantidade = pedidoItem.Quantidade;
            if (PedidoItemExiste(pedidoItem))
            {
                var itemExistente = ObterPedidoItemExistente(pedidoItem.ProdutoId);
                quantidade += itemExistente.Quantidade;
            }

            if (quantidade > MaxUnidadesItems)
                throw new DomainException($"A quantidade máxima para um produto é {MaxUnidadesItems}");
        }

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            ValidarQuantidadeMaximaItem(pedidoItem);

            if (PedidoItemExiste(pedidoItem))
            {
                var itemExistente = ObterPedidoItemExistente(pedidoItem.ProdutoId);
                itemExistente.AdicionarUnidades(pedidoItem.Quantidade);

                pedidoItem = itemExistente;
                _pedidoItens.Remove(itemExistente);
            }

            _pedidoItens.Add(pedidoItem);
            CalcularValorPedido();


        }

        public void AtualizarItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemInexistente(pedidoItem);
            ValidarQuantidadeMaximaItem(pedidoItem);

            var itemExistente = ObterPedidoItemExistente(pedidoItem.ProdutoId);

            _pedidoItens.Remove(itemExistente);
            _pedidoItens.Add(pedidoItem);

            CalcularValorPedido();
        }

        public void RemoverItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemInexistente(pedidoItem);
            var itemExiste = ObterPedidoItemExistente(pedidoItem.ProdutoId);

            _pedidoItens.Remove(itemExiste);
            CalcularValorPedido();
        }

        public void TornarRascuho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }
        public void AtualizarUnidades(PedidoItem item, int unidades)
        {
            item.AtualizarUnidades(unidades);
            AtualizarItem(item);
        }

        public ValidationResult AplicarVoucher(Voucher voucher)
        {
            var result = voucher.ValidarSeAplicavel();
            if (!result.IsValid) return result;

            VoucherUtilizado = true;
            Voucher = voucher;

            CalcularValorPedido();

            return result;
        }

        private void CalcularValorDesconto()
        {
            if (!VoucherUtilizado) return;
            if (Voucher.TipoDescontoVoucher == TipoDescontoVoucher.Valor)
            {
                if (Voucher.ValorDesconto.HasValue)
                    Desconto = Voucher.ValorDesconto.Value;
            }
            else
            {
                if (Voucher.PercentualDesconto.HasValue)
                    Desconto = (Voucher.PercentualDesconto.Value / 100 * ValorTotal);
            }

            ValorTotal -= Desconto;
            if (ValorTotal < 0) ValorTotal = 0;
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido()
                {
                    ClienteId = clienteId
                };

                pedido.TornarRascuho();
                return pedido;
            }
        }
    }
}