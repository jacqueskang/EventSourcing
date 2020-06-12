using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Snapshotting.Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public abstract class AggregateRepository<TAggregate, TKey>
        where TAggregate : class, IAggregate<TKey>
    {
        private readonly IEventStore<TAggregate, TKey> _eventStore;
        private readonly ISnapshotStore<TAggregate, TKey> _snapshotStore;

        protected AggregateRepository(
            IEventStore<TAggregate, TKey> eventStore,
            ISnapshotStore<TAggregate, TKey> snapshotStore)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
        }

        protected virtual async Task<IAggregateChangeset<TKey>> SaveAggregateAsync(TAggregate aggregate,
            CancellationToken cancellationToken = default)
        {
            if (aggregate is null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }

            IAggregateChangeset<TKey> changeset = aggregate.GetChangeset();

            foreach (IAggregateEvent<TKey> @event in changeset.Events)
            {
                await _eventStore.AddEventAsync(@event, cancellationToken).ConfigureAwait(false);
            }

            if (changeset.Snapshot != null)
            {
                await _snapshotStore.AddSnapshotAsync(changeset.Snapshot, cancellationToken).ConfigureAwait(false);
            }

            changeset.Commit();
            return changeset;
        }

        protected virtual Task<TKey[]> GetAggregateIdsAsync()
        {
            return _eventStore.GetAggregateIdsAsync();
        }

        protected virtual async Task<TAggregate> FindAggregateAsync(TKey id,
            bool ignoreSnapshot = false,
            int version = -1,
            CancellationToken cancellationToken = default)
        {
            int maxVersion = version <= 0 ? int.MaxValue : version;

            IAggregateSnapshot<TKey> snapshot = null;
            if (!ignoreSnapshot)
            {
                snapshot = await _snapshotStore
                    .FindLastSnapshotAsync(id, maxVersion,  cancellationToken)
                    .ConfigureAwait(false);
            }

            int minVersion = snapshot == null ? 1 : snapshot.AggregateVersion + 1;
            IAggregateEvent<TKey>[] events = await _eventStore
                .GetEventsAsync(id, minVersion, maxVersion, cancellationToken)
                .ConfigureAwait(false);

            if (snapshot == null)
            {
                return events.Length == 0
                    ? null
                    : Activator.CreateInstance(typeof(TAggregate), id, events as IEnumerable<IAggregateEvent<TKey>>) as TAggregate;
            }
            else
            {
                return Activator.CreateInstance(typeof(TAggregate), id, snapshot, events as IEnumerable<IAggregateEvent<TKey>>) as TAggregate;
            }
        }
    }
}
