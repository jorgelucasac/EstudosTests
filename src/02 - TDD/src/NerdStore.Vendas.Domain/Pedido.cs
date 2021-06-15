using System;
using System.Collections.Generic;
using System.Linq;

namespace NerdStore.Vendas.Domain
{
    public class Pedido
    {
        public const int MaxUnidadesItems = 15;
        public const int MinUnidadesItems = 1;

        public decimal ValorTotal { get; private set; }
        public Guid ClienteId { get; private set; }
        public PedidoStatus PedidoStatus { get; private set; }
        private readonly List<PedidoItem> _pedidoItens;
        public IReadOnlyCollection<PedidoItem> PedidoItens => _pedidoItens.AsReadOnly();

        protected Pedido()
        {
            _pedidoItens = new List<PedidoItem>();
        }

        private void CalcularValorPedido()
        {
            ValorTotal = _pedidoItens.Sum(i => i.CalcularValorTotal());
        }

        public bool PedidoItemExiste(PedidoItem pedidoItem)
        {
            return _pedidoItens.Any(p => p.ProdutoId == pedidoItem.ProdutoId);
        }

        private PedidoItem ObterPedidoItemExistente(Guid produtoId)
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

        public void TornarRascuho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
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