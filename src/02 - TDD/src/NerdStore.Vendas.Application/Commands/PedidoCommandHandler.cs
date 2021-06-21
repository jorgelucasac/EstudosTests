using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NerdStore.Core.DomainObjects;
using NerdStore.Core.Messages;
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
            if (!await ValidarComando(message)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);
            var pedidoItem = new PedidoItem(message.ProdutoId, message.Nome, message.Quantidade, message.ValorUnitario);

            pedido = pedido is null ? AdicionarNovoPedido(message.ClienteId, pedidoItem) : AtualizarPedido(pedido, pedidoItem);


            pedido.AdicionarEvento(new PedidoItemAdicionadoEvent(
                pedido.ClienteId, pedido.Id, message.ProdutoId, message.Nome, message.ValorUnitario, message.Quantidade));



            return await _pedidoRepository.UnitOfWork.Commit();
        }

        private async Task<bool> ValidarComando(Command message)
        {
            if (message.EhValido()) return true;

            foreach (var error in message.ValidationResult.Errors)
            {
               await _mediatr.Publish(new DomainNotification(message.MessageType, error.ErrorMessage, message.AggregateId));
            }

            return false;
        }

        private Pedido AdicionarNovoPedido(Guid clienteId, PedidoItem pedidoItem)
        {
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(clienteId);
            pedido.AdicionarItem(pedidoItem);
            _pedidoRepository.Adicionar(pedido);

            return pedido;
        }

        private Pedido AtualizarPedido(Pedido pedido, PedidoItem pedidoItem)
        {
            var pedidoItemExiste = pedido.PedidoItemExiste(pedidoItem);
            pedido.AdicionarItem(pedidoItem);

            if (pedidoItemExiste)
            {
                _pedidoRepository.AtualizarItem(pedido.ObterPedidoItemExistente(pedidoItem.ProdutoId));
            }
            else
            {
                _pedidoRepository.AdicionarItem(pedidoItem);
            }
            _pedidoRepository.Atualizar(pedido);
            return pedido;
        }
    }
}