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
        private IAggregateSnapshot<TKey> _unsavedSnapshot = null;

        /// <summary>
        /// Create a new aggregate
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

        /// <summary>
        /// Rehydrate an aggregate from historical events
        /// </summary>
        /// <param name="id">Aggregate ID</param>
        /// <param name="savedEvents">Historical events</param>
        protected Aggregate(TKey id,
            IEnumerable<IAggregateEvent<TKey>> savedEvents)
        {
            if (savedEvents is null)
            {
                throw new ArgumentNullException(nameof(savedEvents));
            }

            Id = id;
            foreach (IAggregateEvent<TKey> @event in savedEvents.OrderBy(x => x.AggregateVersion))
            {
                IntegrateEvent(@event);
                _savedEvents.Enqueue(@event);
            }
        }

        /// <summary>
        /// Rehydrate an aggregate from a snapshot + historical events
        /// </summary>
        /// <param name="id">Aggregate ID</param>
        /// <param name="snapshot">Aggregate snapshot</param>
        /// <param name="savedEvents">Historical events after the snapshot</param>
        protected Aggregate(TKey id,
            IAggregateSnapshot<TKey> snapshot,
            IEnumerable<IAggregateEvent<TKey>> savedEvents)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (savedEvents is null)
            {
                throw new ArgumentNullException(nameof(savedEvents));
            }

            Id = id;
            IntegrateSnapshot(snapshot);
            Snapshot = snapshot;

            foreach (IAggregateEvent<TKey> @event in savedEvents.OrderBy(x => x.AggregateVersion))
            {
                IntegrateEvent(@event);
                _savedEvents.Enqueue(@event);
            }
        }

        public TKey Id { get; }

        public int Version { get; private set; } = 0;

        public IEnumerable<IAggregateEvent<TKey>> Events => _savedEvents.Concat(_unsavedEvents);

        public IAggregateSnapshot<TKey> Snapshot { get; private set; } = null;

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

            _unsavedSnapshot = snapshot;
        }

        public IAggregateChangeset<TKey> GetChangeset()
        {
            return new Changeset(this, _unsavedEvents, _unsavedSnapshot);
        }

        protected int GetNextVersion() => Version + 1;

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
                throw new InvalidOperationException($"Cannot integrate event of aggregate #{e.AggregateId} on aggregate #{Id}.");
            }

            if (e.AggregateVersion != GetNextVersion())
            {
                throw new InvalidOperationException($"Cannot integrate event with version '{e.AggregateVersion}' on an aggregate with version '{Version}'");
            }

            ApplyEvent(e);

            Version = e.AggregateVersion;
        }

        private void IntegrateSnapshot(IAggregateSnapshot<TKey> snapshot)
        {
            if (!snapshot.AggregateId.Equals(Id))
            {
                throw new InvalidOperationException($"Cannot integrate snapshot of aggregate #{snapshot.AggregateId} on aggregate #{Id}");
            }

            ApplySnapshot(snapshot);

            Version = snapshot.AggregateVersion;
        }

        internal class Changeset : IAggregateChangeset<TKey>
        {
            private readonly Aggregate<TKey> _aggregate;
            private bool _committed = false;

            public Changeset(
                Aggregate<TKey> aggregate,
                IEnumerable<IAggregateEvent<TKey>> events,
                IAggregateSnapshot<TKey> snapshot = null)
            {
                if (events is null)
                {
                    throw new ArgumentNullException(nameof(events));
                }

                _aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
                Events = events.ToList().AsReadOnly();
                Snapshot = snapshot;
            }

            public IEnumerable<IAggregateEvent<TKey>> Events { get; }
            public IAggregateSnapshot<TKey> Snapshot { get; }

            public void Commit()
            {
                if (_committed)
                {
                    throw new InvalidOperationException("The changeset is already committed");
                }

                for (int i = 0; i < Events.Count(); i++)
                {
                    IAggregateEvent<TKey> @evt = _aggregate._unsavedEvents.Dequeue();
                    _aggregate._savedEvents.Enqueue(@evt);
                }

                if (Snapshot != null)
                {
                    _aggregate.Snapshot = Snapshot;
                    while (_aggregate._savedEvents.Any() && _aggregate._savedEvents.First().AggregateVersion <= Snapshot.AggregateVersion)
                    {
                        _aggregate._savedEvents.Dequeue();
                    }
                }
                _aggregate._unsavedSnapshot = null;
                _committed = true;
            }
        }
    }
}
