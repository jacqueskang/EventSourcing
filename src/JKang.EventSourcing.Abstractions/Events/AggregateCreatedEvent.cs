using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateCreatedEvent : AggregateEvent
    {
        private const int _aggregateVersion = 1;

        protected AggregateCreatedEvent(Guid id, DateTime dateTime, Guid aggregateId)
            : base(id, dateTime, aggregateId, aggregateVersion: _aggregateVersion)
        { }
    }
}
