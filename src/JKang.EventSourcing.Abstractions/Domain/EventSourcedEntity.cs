using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;

namespace JKang.EventSourcing.Domain
{
    public abstract class EventSourcedEntity
    {
        private readonly Queue<IEvent> _pendingEvents = new Queue<IEvent>();

        /// <summary>
        /// Use this constructor to create a new entity
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <param name="created">The creation event</param>
        protected EventSourcedEntity(Guid id, IEvent created)
        {
            Id = id;
            AppendEvent(created);
        }

        public Guid Id { get; }

        protected void AppendEvent(IEvent @event)
        {
            _pendingEvents.Enqueue(@event);
        }

        public IEnumerable<IEvent> GetPendingEvents()
        {
            return _pendingEvents.ToArray();
        }
    }
}
