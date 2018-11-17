using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public abstract class AggregateRepository<TAggregate>
        where TAggregate : class, IAggregate
    {
        private readonly IEventStore<TAggregate> _eventStore;

        protected AggregateRepository(IEventStore<TAggregate> eventStore)
        {
            _eventStore = eventStore;
        }

        protected async Task SaveAggregateAsync(TAggregate aggregate)
        {
            IAggregateChangeset changeset = aggregate.GetChangeset();
            foreach (IAggregateEvent @event in changeset.Events)
            {
                await _eventStore.AddEventAsync(@event);
                await OnEventSavedAsync(@event);
            }
            changeset.Commit();
        }

        protected virtual Task OnEventSavedAsync(IAggregateEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task<Guid[]> GetAggregateIdsAsync()
        {
            return _eventStore.GetAggregateIdsAsync();
        }

        public async Task<TAggregate> FindAggregateAsync(Guid id)
        {
            IAggregateEvent[] events = await _eventStore.GetEventsAsync(id);
            return events.Length == 0
                ? null
                : Activator.CreateInstance(typeof(TAggregate), id, events as IEnumerable<IAggregateEvent>) as TAggregate;
        }
    }
}
