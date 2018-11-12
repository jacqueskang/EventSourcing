using JKang.EventSourcing.Events;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public class DefaultEventStore : IEventStore
    {
        private readonly IEventSerializer _eventSerializer;
        private readonly IBinaryStore _binaryStore;

        public DefaultEventStore(
            IEventSerializer eventSerializer,
            IBinaryStore binaryStore)
        {
            _eventSerializer = eventSerializer;
            _binaryStore = binaryStore;
        }

        public async Task SaveEventAsync(Guid entityId, IEvent @event)
        {
            byte[] serialized = _eventSerializer.Serialize(@event);
            await _binaryStore.SaveAsync(entityId.ToString(), @event.Id.ToString(), serialized);
        }
    }
}
