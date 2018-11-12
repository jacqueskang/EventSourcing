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
            ReceiveEvent(created);
        }

        protected EventSourcedEntity(Guid id, IEnumerable<IEvent> history)
        {
            Id = id;
            foreach (IEvent @event in history)
            {
                ProcessEvent(@event);
            }
        }

        public Guid Id { get; }

        protected abstract void ProcessEvent(IEvent @event);

        protected void ReceiveEvent(IEvent @event)
        {
            ProcessEvent(@event);
            _pendingEvents.Enqueue(@event);
        }

        public IEnumerable<IEvent> GetPendingEvents()
        {
            return _pendingEvents.ToArray();
        }

        public void ClearPendingEvents()
        {
            _pendingEvents.Clear();
        }
    }
}
