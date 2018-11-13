using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStore<TEventSourcedAggregate>
        where TEventSourcedAggregate : EventSourcedAggregate
    {
        Task AddEventAsync(AggregateEvent @event);
        Task<AggregateEvent[]> GetEventsAsync(Guid aggregateId);
        Task<Guid[]> GetAggregateIdsAsync();
    }
}
