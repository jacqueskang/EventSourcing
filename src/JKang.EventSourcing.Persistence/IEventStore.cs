using JKang.EventSourcing.Events;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStore
    {
        Task AddEventAsync(string entityType, Guid entityId, IEvent @event);
        Task<IEvent[]> GetEventsAsync(string entityType, Guid entityId);
        Task<Guid[]> GetEntityIdsAsync(string entityType);
    }
}
