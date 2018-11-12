using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public abstract class EventSourcedEntityRepository<TEventSourcedEntity>
        where TEventSourcedEntity : EventSourcedEntity
    {
        private readonly IEventStore _eventStore;

        protected EventSourcedEntityRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        protected async Task CreateEntityAsync(TEventSourcedEntity entity)
        {
            IEnumerable<IEvent> pendingEvents = entity.GetPendingEvents();
            foreach (IEvent @event in pendingEvents)
            {
                await _eventStore.AddEventAsync(entity.Id, @event);
            }
            entity.ClearPendingEvents();
        }

        public async Task<TEventSourcedEntity> FindEntityAsync(Guid id)
        {
            IEnumerable<IEvent> events = await _eventStore.GetEventsAsync(id);
            var entity = (TEventSourcedEntity)Activator.CreateInstance(typeof(TEventSourcedEntity), id, events);
            return entity;
        }
    }
}
