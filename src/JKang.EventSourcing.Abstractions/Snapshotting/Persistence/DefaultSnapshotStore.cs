using JKang.EventSourcing.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Snapshotting.Persistence
{
    public class DefaultSnapshotStore<TAggregate, TKey>
        : ISnapshotStore<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        public Task AddSnapshotAsync(IAggregateSnapshot<TKey> snapshot, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IAggregateSnapshot<TKey>> FindLastSnapshotAsync(TKey aggregateId, int maxVersion,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(null as IAggregateSnapshot<TKey>);
        }
    }
}
