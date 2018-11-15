using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JKang.EventSourcing.Domain
{
    public abstract class Aggregate: IAggregate
    {
        private readonly Queue<IAggregateEvent> _savedEvents = new Queue<IAggregateEvent>();
        private readonly Queue<IAggregateEvent> _unsavedEvents = new Queue<IAggregateEvent>();

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

        protected Aggregate(Guid id, IEnumerable<IAggregateEvent> savedEvents)
        {
            Id = id;
            foreach (IAggregateEvent @event in savedEvents.OrderBy(x => x.AggregateVersion))
            {
                IntegrateEvent(@event);
                _savedEvents.Enqueue(@event);
            }
        }

        public Guid Id { get; }

        public int Version { get; private set; } = 0;

        protected int GetNextVersion() => Version + 1;

        public IEnumerable<IAggregateEvent> Events { get => _savedEvents.Concat(_unsavedEvents); }

        public IAggregateChangeset GetChangeset()
        {
            return new Changeset(_unsavedEvents, this);
        }

        protected abstract void ApplyEvent(IAggregateEvent @event);

        protected void ReceiveEvent(IAggregateEvent @event)
        {
            IntegrateEvent(@event);
            _unsavedEvents.Enqueue(@event);
        }

        private void IntegrateEvent(IAggregateEvent @event)
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

        public class Changeset: IAggregateChangeset
        {
            private readonly Aggregate _aggregate;
            public Changeset(IEnumerable<IAggregateEvent> events, Aggregate aggregate)
            {
                Events = events.ToList().AsReadOnly();
                _aggregate = aggregate;
            }

            public IEnumerable<IAggregateEvent> Events { get; }

            public void Commit()
            {
                for (int i = 0; i < Events.Count(); i++)
                {
                    IAggregateEvent @evt = _aggregate._unsavedEvents.Dequeue();
                    _aggregate._savedEvents.Enqueue(@evt);
                }
            }
        }
    }
}
