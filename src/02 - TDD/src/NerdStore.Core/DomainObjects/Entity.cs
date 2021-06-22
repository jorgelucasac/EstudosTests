using System;
using System.Collections.Generic;
using NerdStore.Core.Messages;

namespace NerdStore.Core.DomainObjects
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public IReadOnlyCollection<Event> Notificacoes => _notificacoes.AsReadOnly();
        private readonly List<Event> _notificacoes;


        protected Entity()
        {
            Id = Guid.NewGuid();
            _notificacoes = new List<Event>();
        }

        public void AdicionarEvento(Event evento)
        {
            _notificacoes.Add(evento);
        }

        public void RemoverEvento(Event eventItem)
        {
            _notificacoes?.Remove(eventItem);
        }

        public void LimparEventos()
        {
            _notificacoes?.Clear();
        }


    }
}