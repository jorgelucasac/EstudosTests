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
    public class PedidoCommandHandler : 
        IRequestHandler<AdicionarItemPedidoCommand, bool>,
    IRequestHandler<AtualizarItemPedidoCommand, bool>,
    IRequestHandler<RemoverItemPedidoCommand, bool>,
    IRequestHandler<AplicarVoucherPedidoCommand, bool>
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

        public async Task<bool> Handle(AtualizarItemPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!await ValidarComando(message)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);

            if (pedido == null)
            {
                await _mediatr.Publish(new DomainNotification("pedido", "Pedido não encontrado!"), cancellationToken);
                return false;
            }

            var pedidoItem = await _pedidoRepository.ObterItemPorPedido(pedido.Id, message.ProdutoId);

            if (!pedido.PedidoItemExiste(pedidoItem))
            {
                await _mediatr.Publish(new DomainNotification("pedido", "Item do pedido não encontrado!"), cancellationToken);
                return false;
            }

            pedido.AtualizarUnidades(pedidoItem, message.Quantidade);
            pedido.AdicionarEvento(new PedidoProdutoAtualizadoEvent(message.ClienteId, pedido.Id, message.ProdutoId, message.Quantidade));

            _pedidoRepository.AtualizarItem(pedidoItem);
            _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(RemoverItemPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!await ValidarComando(message)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);

            if (pedido == null)
            {
                await _mediatr.Publish(new DomainNotification("pedido", "Pedido não encontrado!"), cancellationToken);
                return false;
            }

            var pedidoItem = await _pedidoRepository.ObterItemPorPedido(pedido.Id, message.ProdutoId);

            if (pedidoItem != null && !pedido.PedidoItemExiste(pedidoItem))
            {
                await _mediatr.Publish(new DomainNotification("pedido", "Item do pedido não encontrado!"), cancellationToken);
                return false;
            }

            pedido.RemoverItem(pedidoItem);
            pedido.AdicionarEvento(new PedidoProdutoRemovidoEvent(message.ClienteId, pedido.Id, message.ProdutoId));

            _pedidoRepository.RemoverItem(pedidoItem);
            _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(AplicarVoucherPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!await ValidarComando(message)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);

            if (pedido == null)
            {
                await _mediatr.Publish(new DomainNotification("pedido", "Pedido não encontrado!"), cancellationToken);
                return false;
            }

            var voucher = await _pedidoRepository.ObterVoucherPorCodigo(message.CodigoVoucher);

            if (voucher == null)
            {
                await _mediatr.Publish(new DomainNotification("pedido", "Voucher não encontrado!"), cancellationToken);
                return false;
            }

            var voucherAplicacaoValidation = pedido.AplicarVoucher(voucher);
            if (!voucherAplicacaoValidation.IsValid)
            {
                foreach (var error in voucherAplicacaoValidation.Errors)
                {
                    await _mediatr.Publish(new DomainNotification(error.ErrorCode, error.ErrorMessage), cancellationToken);
                }

                return false;
            }

            pedido.AdicionarEvento(new VoucherAplicadoPedidoEvent(message.ClienteId, pedido.Id, voucher.Id));

            _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
        }
    }
}