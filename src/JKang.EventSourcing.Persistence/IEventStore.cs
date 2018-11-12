using JKang.EventSourcing.Events;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStore
    {
        Task AddEventAsync(Guid entityId, IEvent @event);
        Task<IEvent[]> GetEventsAsync(Guid entityId);
    }
}
