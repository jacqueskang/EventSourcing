using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JKang.EventSourcing.Domain
{
    public abstract class EventSourcedAggregate
    {
        private readonly Queue<IEvent> _savedEvents = new Queue<IEvent>();
        private readonly Queue<IEvent> _unsavedEvents = new Queue<IEvent>();

        /// <summary>
        /// Use this constructor to create a new aggregate
        /// </summary>
        /// <param name="id">Aggregate ID</param>
        /// <param name="created">The creation event</param>
        protected EventSourcedAggregate(Guid id, IEvent created)
        {
            Id = id;
            ReceiveEvent(created);
        }

        protected EventSourcedAggregate(Guid id, IEnumerable<IEvent> savedEvents)
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
            private readonly EventSourcedAggregate _aggregate;
            public Changeset(IEnumerable<IEvent> events, EventSourcedAggregate aggregate)
            {
                Events = events.ToList().AsReadOnly();
                _aggregate = aggregate;
            }

            public ReadOnlyCollection<IEvent> Events { get; }

            public void Commit()
            {
                for (int i = 0; i < Events.Count; i++)
                {
                    IEvent @evt = _aggregate._unsavedEvents.Dequeue();
                    _aggregate._savedEvents.Enqueue(@evt);
                }
            }
        }
    }
}
