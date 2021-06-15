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

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            if (pedidoItem.Quantidade > MaxUnidadesItems)
                throw new DomainException($"A quantidade máxima para um produto é {MaxUnidadesItems}");


            var itemExistente = _pedidoItens.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);
            if (itemExistente != null)
            {
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