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
        private readonly string _entityType;

        protected EventSourcedEntityRepository(IEventStore eventStore)
            : this(eventStore, typeof(TEventSourcedEntity).FullName)
        { }

        protected EventSourcedEntityRepository(IEventStore eventStore, string entityType)
        {
            _eventStore = eventStore;
            _entityType = entityType;
        }

        protected async Task CreateEntityAsync(TEventSourcedEntity entity)
        {
            IEnumerable<IEvent> pendingEvents = entity.GetPendingEvents();
            foreach (IEvent @event in pendingEvents)
            {
                await _eventStore.AddEventAsync(_entityType, entity.Id, @event);
            }
            entity.ClearPendingEvents();
        }

        public Task<Guid[]> GetEntityIdsAsync()
        {
            return _eventStore.GetEntityIdsAsync(_entityType);
        }

        public async Task<TEventSourcedEntity> FindEntityAsync(Guid id)
        {
            IEnumerable<IEvent> events = await _eventStore.GetEventsAsync(_entityType, id);
            var entity = (TEventSourcedEntity)Activator.CreateInstance(typeof(TEventSourcedEntity), id, events);
            return entity;
        }
    }
}
