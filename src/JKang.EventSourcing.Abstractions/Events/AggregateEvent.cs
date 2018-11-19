using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateEvent<TAggregateKey> : IAggregateEvent<TAggregateKey>
    {
        protected AggregateEvent(Guid id, DateTime dateTime, TAggregateKey aggregateId, int aggregateVersion)
        {
            Id = id;
            DateTime = dateTime;
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
        }

        public Guid Id { get; }
        public DateTime DateTime { get; }
        public TAggregateKey AggregateId { get; }
        public int AggregateVersion { get; }
    }
}
