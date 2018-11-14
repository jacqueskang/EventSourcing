using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateCreatedEvent : AggregateEvent
    {
        protected AggregateCreatedEvent(Guid id, DateTime dateTime, Guid aggregateId)
            : base(id, dateTime, aggregateId, aggregateVersion: 1)
        { }
    }
}
