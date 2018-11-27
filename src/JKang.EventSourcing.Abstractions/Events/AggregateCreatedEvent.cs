using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateCreatedEvent<TAggregateKey> : AggregateEvent<TAggregateKey>
    {
        protected AggregateCreatedEvent(TAggregateKey aggregateId, DateTime timestamp)
            : base(aggregateId, aggregateVersion: 1, timestamp: timestamp)
        { }
    }
}
