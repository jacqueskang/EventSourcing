using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JKang.EventSourcing.Domain
{
    public abstract class Aggregate<TKey> : IAggregate<TKey>
    {
        private readonly Queue<IAggregateEvent<TKey>> _savedEvents = new Queue<IAggregateEvent<TKey>>();
        private readonly Queue<IAggregateEvent<TKey>> _unsavedEvents = new Queue<IAggregateEvent<TKey>>();

        /// <summary>
        /// Use this constructor to create a new aggregate
        /// </summary>
        /// <param name="id">Aggregate ID</param>
        /// <param name="created">The creation event</param>
        protected Aggregate(AggregateCreatedEvent<TKey> created)
        {
            Id = created.AggregateId;
            ReceiveEvent(created);
        }

        protected Aggregate(TKey id, IEnumerable<IAggregateEvent<TKey>> savedEvents)
        {
            Id = id;
            foreach (IAggregateEvent<TKey> @event in savedEvents.OrderBy(x => x.AggregateVersion))
            {
                IntegrateEvent(@event);
                _savedEvents.Enqueue(@event);
            }
        }

        public TKey Id { get; }

        public int Version { get; private set; } = 0;

        protected int GetNextVersion() => Version + 1;

        public IEnumerable<IAggregateEvent<TKey>> Events { get => _savedEvents.Concat(_unsavedEvents); }

        public IAggregateChangeset<TKey> GetChangeset()
        {
            return new Changeset(_unsavedEvents, this);
        }

        protected abstract void ApplyEvent(IAggregateEvent<TKey> @event);

        protected void ReceiveEvent(IAggregateEvent<TKey> @event)
        {
            IntegrateEvent(@event);
            _unsavedEvents.Enqueue(@event);
        }

        private void IntegrateEvent(IAggregateEvent<TKey> @event)
        {
            if (!@event.AggregateId.Equals(Id))
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

        public class Changeset : IAggregateChangeset<TKey>
        {
            private readonly Aggregate<TKey> _aggregate;
            public Changeset(IEnumerable<IAggregateEvent<TKey>> events, Aggregate<TKey> aggregate)
            {
                Events = events.ToList().AsReadOnly();
                _aggregate = aggregate;
            }

            public IEnumerable<IAggregateEvent<TKey>> Events { get; }

            public void Commit()
            {
                for (int i = 0; i < Events.Count(); i++)
                {
                    IAggregateEvent<TKey> @evt = _aggregate._unsavedEvents.Dequeue();
                    _aggregate._savedEvents.Enqueue(@evt);
                }
            }
        }
    }
}
