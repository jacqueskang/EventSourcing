using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public abstract class EventSourcedAggregateRepository<TEventSourcedAggregate>
        where TEventSourcedAggregate : EventSourcedAggregate
    {
        private readonly IEventStore<TEventSourcedAggregate> _eventStore;

        public event EventHandler<AggregateSavedEventArgs> AggregateSaved;

        protected EventSourcedAggregateRepository(IEventStore<TEventSourcedAggregate> eventStore)
        {
            _eventStore = eventStore;
        }

        protected async Task SaveAggregateAsync(TEventSourcedAggregate aggregate)
        {
            EventSourcedAggregate.Changeset changeset = aggregate.GetChangeset();
            foreach (AggregateEvent @event in changeset.Events)
            {
                await _eventStore.AddEventAsync(@event);
            }
            changeset.Commit();

            AggregateSaved?.Invoke(this, new AggregateSavedEventArgs(changeset.Events));
        }

        public Task<Guid[]> GetAggregateIdsAsync()
        {
            return _eventStore.GetAggregateIdsAsync();
        }

        public async Task<TEventSourcedAggregate> FindAggregateAsync(Guid id)
        {
            AggregateEvent[] events = await _eventStore.GetEventsAsync(id);
            return events.Length == 0
                ? null
                : Activator.CreateInstance(typeof(TEventSourcedAggregate), id, events as IEnumerable<AggregateEvent>) as TEventSourcedAggregate;
        }
    }
}
