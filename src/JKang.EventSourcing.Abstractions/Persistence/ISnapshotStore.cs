using JKang.EventSourcing.Caching;
using JKang.EventSourcing.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface ISnapshotStore<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        Task AddSnapshotAsync(IAggregateSnapshot<TKey> snapshot,
            CancellationToken cancellationToken = default);

        Task<IAggregateSnapshot<TKey>> FindLastSnapshotAsync(TKey aggregateId,
            CancellationToken cancellationToken = default);
    }
}
