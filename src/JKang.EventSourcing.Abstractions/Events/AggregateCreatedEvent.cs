using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateCreatedEvent<TAggregateKey> : AggregateEvent<TAggregateKey>
    {
        protected AggregateCreatedEvent(Guid id, DateTime dateTime, TAggregateKey aggregateId)
            : base(id, dateTime, aggregateId, aggregateVersion: 1)
        { }
    }
}
