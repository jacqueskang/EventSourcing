using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JKang.EventSourcing.Domain
{
    public abstract class Aggregate
    {
        private readonly Queue<AggregateEvent> _savedEvents = new Queue<AggregateEvent>();
        private readonly Queue<AggregateEvent> _unsavedEvents = new Queue<AggregateEvent>();

        /// <summary>
        /// Use this constructor to create a new aggregate
        /// </summary>
        /// <param name="id">Aggregate ID</param>
        /// <param name="created">The creation event</param>
        protected Aggregate(AggregateCreatedEvent created)
        {
            Id = created.AggregateId;
            ReceiveEvent(created);
        }

        protected Aggregate(Guid id, IEnumerable<AggregateEvent> savedEvents)
        {
            Id = id;
            foreach (AggregateEvent @event in savedEvents.OrderBy(x => x.AggregateVersion))
            {
                IntegrateEvent(@event);
                _savedEvents.Enqueue(@event);
            }
        }

        public Guid Id { get; }

        public int Version { get; private set; } = 0;

        protected int GetNextVersion() => Version + 1;

        public IEnumerable<AggregateEvent> Events { get => _savedEvents.Concat(_unsavedEvents); }

        public Changeset GetChangeset()
        {
            return new Changeset(_unsavedEvents, this);
        }

        protected abstract void ApplyEvent(AggregateEvent @event);

        protected void ReceiveEvent(AggregateEvent @event)
        {
            IntegrateEvent(@event);
            _unsavedEvents.Enqueue(@event);
        }

        private void IntegrateEvent(AggregateEvent @event)
        {
            if (@event.AggregateId != Id)
            {
                throw new InvalidOperationException($"Cannot integrate event with {nameof(@event.AggregateId)} '{@event.AggregateId}' on an aggregate with {nameof(Id)} '{Id}'");
            }

            if (@event.AggregateVersion != GetNextVersion())
            {
                throw new InvalidOperationException($"Cannot integrate event with {nameof(@event.AggregateVersion)} '{@event.AggregateVersion}' on an aggregate with {nameof(Version)} '{Version}'");
            }

            ApplyEvent(@event);

            Version = @event.AggregateVersion;
        }

        public class Changeset
        {
            private readonly Aggregate _aggregate;
            public Changeset(IEnumerable<AggregateEvent> events, Aggregate aggregate)
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
