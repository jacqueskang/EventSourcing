using JKang.EventSourcing.Events;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStore
    {
        Task AddEventAsync(string aggregateType, Guid aggregateId, IEvent @event);
        Task<IEvent[]> GetEventsAsync(string aggregateType, Guid aggregateId);
        Task<Guid[]> GetAggregateIdsAsync(string aggregateType);
    }
}
