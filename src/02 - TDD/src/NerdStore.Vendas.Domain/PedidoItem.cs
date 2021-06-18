﻿using System;

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
            if (quantidade < Pedido.MinUnidadesItems)
                throw new DomainException($"A quantidade mínima para um produto é {Pedido.MinUnidadesItems}");


            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }

        internal decimal CalcularValorTotal()
        {
            return Quantidade * ValorUnitario;
        }

        internal void AdicionarUnidades(int quantidade)
        {
            Quantidade += quantidade;
        }
    }
}