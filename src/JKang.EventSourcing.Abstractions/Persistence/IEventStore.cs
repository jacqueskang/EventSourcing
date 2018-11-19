using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStore<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
    {
        Task AddEventAsync(IAggregateEvent<TAggregateKey> @event);
        Task<IAggregateEvent<TAggregateKey>[]> GetEventsAsync(TAggregateKey aggregateId);
        Task<TAggregateKey[]> GetAggregateIdsAsync();
    }
}
