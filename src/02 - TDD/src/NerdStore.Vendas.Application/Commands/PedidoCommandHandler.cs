using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NerdStore.Vendas.Application.Events;
using NerdStore.Vendas.Domain;

namespace NerdStore.Vendas.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarItemPedidoCommand, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMediator _mediatr;

        public PedidoCommandHandler(IPedidoRepository pedidoRepository, IMediator mediatr)
        {
            _pedidoRepository = pedidoRepository;
            _mediatr = mediatr;
        }



        public async Task<bool> Handle(AdicionarItemPedidoCommand message, CancellationToken cancellationToken)
        {
            var pedidoItem = new PedidoItem(message.ProdutoId, message.Nome, message.Quantidade, message.ValorUnitario);
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(message.ClienteId);
            pedido.AdicionarItem(pedidoItem);

            pedido.AdicionarEvento(new PedidoItemAdicionadoEvent(
                pedido.ClienteId, pedido.Id, message.ProdutoId, message.Nome, message.ValorUnitario, message.Quantidade));
            
            _pedidoRepository.Adicionar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
        }
    }
}