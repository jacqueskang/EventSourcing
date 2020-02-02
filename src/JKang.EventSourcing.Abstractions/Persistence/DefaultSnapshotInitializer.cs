using JKang.EventSourcing.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public class DefaultSnapshotInitializer<TAggregate, TKey>
        : ISnapshotStoreInitializer<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        public Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
