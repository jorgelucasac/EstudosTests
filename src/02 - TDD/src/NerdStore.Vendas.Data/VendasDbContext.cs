using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NerdStore.Core.Data;

namespace NerdStore.Vendas.Data
{
    public class VendasDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public VendasDbContext(DbContextOptions<VendasDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public async Task<bool> Commit()
        {
            var sucesso = await SaveChangesAsync() > 0;
            if (sucesso)
                await _mediator.PublicarEventos(this);
            return sucesso;
        }


    }
}
