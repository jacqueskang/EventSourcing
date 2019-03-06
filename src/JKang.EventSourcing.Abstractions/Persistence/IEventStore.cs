using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStore<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
    {
        Task AddEventAsync(IAggregateEvent<TAggregateKey> @event,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IAggregateEvent<TAggregateKey>[]> GetEventsAsync(TAggregateKey aggregateId,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TAggregateKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
