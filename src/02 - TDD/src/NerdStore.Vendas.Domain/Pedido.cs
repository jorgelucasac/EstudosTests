using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NerdStore.Vendas.Domain
{
    public class Pedido
    {
        public decimal ValorTotal { get; private set; }
        private readonly List<PedidoItem> _pedidoItens;
        public IReadOnlyCollection<PedidoItem> PedidoItens => _pedidoItens.AsReadOnly();

        public Pedido()
        {
            _pedidoItens = new List<PedidoItem>();
        }

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            _pedidoItens.Add(pedidoItem);
            ValorTotal = _pedidoItens.Sum(i=> i.CalcularValorTotal());
        }
    }
}