using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateCreatedEvent<TKey> : AggregateEvent<TKey>
    {
        protected AggregateCreatedEvent(TKey aggregateId, DateTime timestamp)
            : base(aggregateId, aggregateVersion: 1, timestamp: timestamp)
        { }
    }
}
