using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JKang.EventSourcing.Domain
{
    public abstract class EventSourcedAggregate
    {
        private readonly Queue<AggregateEvent> _savedEvents = new Queue<AggregateEvent>();
        private readonly Queue<AggregateEvent> _unsavedEvents = new Queue<AggregateEvent>();

        /// <summary>
        /// Use this constructor to create a new aggregate
        /// </summary>
        /// <param name="id">Aggregate ID</param>
        /// <param name="created">The creation event</param>
        protected EventSourcedAggregate(Guid id, AggregateEvent created)
        {
            Id = id;
            ReceiveEvent(created);
        }

        protected EventSourcedAggregate(Guid id, IEnumerable<AggregateEvent> savedEvents)
        {
            Id = id;
            foreach (AggregateEvent @event in savedEvents)
            {
                ProcessEvent(@event);
                _savedEvents.Enqueue(@event);
            }
        }

        public Guid Id { get; }

        public int Version { get; protected set; }

        public IEnumerable<AggregateEvent> Events { get => _savedEvents.Concat(_unsavedEvents); }

        public Changeset GetChangeset()
        {
            return new Changeset(_unsavedEvents, this);
        }

        protected abstract void ProcessEvent(AggregateEvent @event);

        protected void ReceiveEvent(AggregateEvent @event)
        {
            ProcessEvent(@event);
            _unsavedEvents.Enqueue(@event);
        }

        public class Changeset
        {
            private readonly EventSourcedAggregate _aggregate;
            public Changeset(IEnumerable<AggregateEvent> events, EventSourcedAggregate aggregate)
            {
                Events = events.ToList().AsReadOnly();
                _aggregate = aggregate;
            }

            public ReadOnlyCollection<AggregateEvent> Events { get; }

            public void Commit()
            {
                for (int i = 0; i < Events.Count; i++)
                {
                    AggregateEvent @evt = _aggregate._unsavedEvents.Dequeue();
                    _aggregate._savedEvents.Enqueue(@evt);
                }
            }
        }
    }
}
