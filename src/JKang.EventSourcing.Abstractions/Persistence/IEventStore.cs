using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStore<TAggregate>
        where TAggregate : IAggregate
    {
        Task AddEventAsync(IAggregateEvent @event);
        Task<IAggregateEvent[]> GetEventsAsync(Guid aggregateId);
        Task<Guid[]> GetAggregateIdsAsync();
    }
}
