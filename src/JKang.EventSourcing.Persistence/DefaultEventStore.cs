using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEvent[]> GetEventsAsync(string entityType, Guid entityId)
        {
            string container = entityId.ToString();
            string[] keys = await _binaryStore.GetKeysInContainerAsync(entityType, container);
            var events = new List<IEvent>();
            foreach (string key in keys)
            {
                byte[] serialized = await _binaryStore.GetDataAsync(entityType, container, key);
                IEvent @event = _eventSerializer.Deserialize(serialized);
                events.Add(@event);
            }
            return events.ToArray();
        }

        public async Task AddEventAsync(string entityType, Guid entityId, IEvent @event)
        {
            byte[] serialized = _eventSerializer.Serialize(@event);
            await _binaryStore.SaveAsync(entityType, entityId.ToString(), @event.Id.ToString(), serialized);
        }

        public async Task<Guid[]> GetEntityIdsAsync(string entityType)
        {
            string[] containers = await _binaryStore.GetContainersInStoreAsync(entityType);
            return containers.Select(x => Guid.Parse(x)).ToArray();
        }
    }
}
