using JKang.EventSourcing.Events;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStore
    {
        Task SaveEventAsync(Guid entityId, IEvent @event);
    }
}
