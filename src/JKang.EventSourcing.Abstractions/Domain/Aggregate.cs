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
        protected Aggregate(IAggregateEvent<TKey> created)
        {
            if (created is null)
            {
                throw new ArgumentNullException(nameof(created));
            }

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

        public void TakeSnapshot()
        {
            IAggregateSnapshot<TKey> snapshot = CreateSnapshot();

            if (!Id.Equals(snapshot.AggregateId))
            {
                throw new InvalidOperationException($"Snapshot AggregateId must be {Id}");
            }

            if (!Version.Equals(snapshot.AggregateVersion))
            {
                throw new InvalidOperationException($"Snapshot AggregateVersion must be {Version}");
            }

            if (!Events.Last().Timestamp.Equals(snapshot.Timestamp))
            {
                throw new InvalidOperationException($"Snapshot AggregateVersion must be {Events.Last().Timestamp}");
            }

            _unsavedEvents.Enqueue(snapshot);
        }

        public IAggregateChangeset<TKey> GetChangeset()
        {
            return new Changeset(_unsavedEvents, this);
        }

        protected abstract void ApplyEvent(IAggregateEvent<TKey> e);

        protected virtual void ApplySnapshot(IAggregateSnapshot<TKey> snapshot) => throw new NotImplementedException();

        protected virtual IAggregateSnapshot<TKey> CreateSnapshot() => throw new NotImplementedException();

        protected void ReceiveEvent(IAggregateEvent<TKey> e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            IntegrateEvent(e);
            _unsavedEvents.Enqueue(e);
        }

        private void IntegrateEvent(IAggregateEvent<TKey> e)
        {
            if (!e.AggregateId.Equals(Id))
            {
                throw new InvalidOperationException($"Cannot integrate event with {nameof(e.AggregateId)} '{e.AggregateId}' on an aggregate with {nameof(Id)} '{Id}'");
            }

            if (e is IAggregateSnapshot<TKey> snapshot)
            {
                ApplySnapshot(snapshot);
            }
            else
            {
                if (e.AggregateVersion != GetNextVersion())
                {
                    throw new InvalidOperationException($"Cannot integrate event with {nameof(e.AggregateVersion)} '{e.AggregateVersion}' on an aggregate with {nameof(Version)} '{Version}'");
                }

                ApplyEvent(e);
            }

            Version = e.AggregateVersion;
        }

        internal class Changeset : IAggregateChangeset<TKey>
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
