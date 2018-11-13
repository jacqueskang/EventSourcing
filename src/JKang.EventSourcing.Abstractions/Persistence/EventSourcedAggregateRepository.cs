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
        private readonly IEventStore _eventStore;
        private readonly string _aggregateType;

        public event EventHandler<AggregateSavedEventArgs> AggregateSaved;

        protected EventSourcedAggregateRepository(IEventStore eventStore)
            : this(eventStore, typeof(TEventSourcedAggregate).FullName)
        { }

        protected EventSourcedAggregateRepository(IEventStore eventStore, string aggregateType)
        {
            _eventStore = eventStore;
            _aggregateType = aggregateType;
        }

        protected async Task SaveAggregateAsync(TEventSourcedAggregate aggregate)
        {
            EventSourcedAggregate.Changeset changeset = aggregate.GetChangeset();
            foreach (IEvent @event in changeset.Events)
            {
                await _eventStore.AddEventAsync(_aggregateType, aggregate.Id, @event);
            }
            changeset.Commit();

            AggregateSaved?.Invoke(this, new AggregateSavedEventArgs(changeset.Events));
        }

        public Task<Guid[]> GetAggregateIdsAsync()
        {
            return _eventStore.GetAggregateIdsAsync(_aggregateType);
        }

        public async Task<TEventSourcedAggregate> FindAggregateAsync(Guid id)
        {
            IEvent[] events = await _eventStore.GetEventsAsync(_aggregateType, id);
            return events.Length == 0
                ? null
                : Activator.CreateInstance(typeof(TEventSourcedAggregate), id, events as IEnumerable<IEvent>) as TEventSourcedAggregate;
        }
    }
}
