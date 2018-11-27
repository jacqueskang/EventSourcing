using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateEvent<TAggregateKey> : IAggregateEvent<TAggregateKey>
    {
        protected AggregateEvent(TAggregateKey aggregateId, int aggregateVersion, DateTime timestamp)
        {
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
            Timestamp = timestamp;
        }

        public TAggregateKey AggregateId { get; }
        public int AggregateVersion { get; }
        public DateTime Timestamp { get; }
    }
}