using JKang.EventSourcing.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Snapshotting.Persistence
{
    public interface ISnapshotStoreInitializer<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        Task EnsureCreatedAsync(CancellationToken cancellationToken = default);
    }
}
