using System;

namespace NerdStore.Vendas.Domain
{
    public class PedidoItem
    {
        public Guid ProdutoId { get; private set; }
        public string ProdutoNome { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }

        public PedidoItem(Guid produtoId, string produtoNome, int quantidade, decimal valorUnitario)
        {
            ProdutoId = produtoId;
            this.ProdutoNome = produtoNome;
            this.Quantidade = quantidade;
            this.ValorUnitario = valorUnitario;
        }

        internal decimal CalcularValorTotal()
        {
            return Quantidade * ValorUnitario;
        }
    }
}