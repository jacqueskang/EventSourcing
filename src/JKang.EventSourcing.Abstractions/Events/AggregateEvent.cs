using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateEvent : Event
    {
        protected AggregateEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        protected AggregateEvent(Guid id, DateTime dateTime, Guid aggregateId)
            : base(id, dateTime)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
    }
}
