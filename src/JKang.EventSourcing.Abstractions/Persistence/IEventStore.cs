using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStore<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        Task AddEventAsync(IAggregateEvent<TKey> e,
            CancellationToken cancellationToken = default);

        Task<IAggregateEvent<TKey>[]> GetEventsAsync(TKey aggregateId,
            int skip = 0,
            CancellationToken cancellationToken = default);

        Task<TKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default);
    }
}
