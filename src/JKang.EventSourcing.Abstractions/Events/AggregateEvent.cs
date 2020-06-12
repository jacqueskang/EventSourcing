using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateEvent<TKey> : IAggregateEvent<TKey>
    {
        protected AggregateEvent(TKey aggregateId, int aggregateVersion, DateTime timestamp)
        {
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
            Timestamp = timestamp;
        }

        public TKey AggregateId { get; }
        public int AggregateVersion { get; }
        public DateTime Timestamp { get; }
    }
}