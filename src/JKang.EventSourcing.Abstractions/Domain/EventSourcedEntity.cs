using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JKang.EventSourcing.Domain
{
    public abstract class EventSourcedEntity
    {
        private readonly Queue<IEvent> _savedEvents = new Queue<IEvent>();
        private readonly Queue<IEvent> _unsavedEvents = new Queue<IEvent>();

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

        protected EventSourcedEntity(Guid id, IEnumerable<IEvent> savedEvents)
        {
            Id = id;
            foreach (IEvent @event in savedEvents)
            {
                ProcessEvent(@event);
                _savedEvents.Enqueue(@event);
            }
        }

        public Guid Id { get; }

        public IEnumerable<IEvent> Events { get => _savedEvents.Concat(_unsavedEvents); }

        public Changeset GetChangeset()
        {
            return new Changeset(_unsavedEvents, this);
        }

        protected abstract void ProcessEvent(IEvent @event);

        protected void ReceiveEvent(IEvent @event)
        {
            ProcessEvent(@event);
            _unsavedEvents.Enqueue(@event);
        }

        public class Changeset
        {
            private readonly EventSourcedEntity _entity;
            public Changeset(IEnumerable<IEvent> events, EventSourcedEntity entity)
            {
                Events = events.ToList().AsReadOnly();
                _entity = entity;
            }

            public ReadOnlyCollection<IEvent> Events { get; }

            public void Commit()
            {
                for (int i = 0; i < Events.Count; i++)
                {
                    IEvent @evt = _entity._unsavedEvents.Dequeue();
                    _entity._savedEvents.Enqueue(@evt);
                }
            }
        }
    }
}
