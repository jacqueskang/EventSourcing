using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Snapshotting.Persistence
{
    public class DefaultSnapshotStoreInitializer<TAggregate, TKey>
        : ISnapshotStoreInitializer<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        public Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
