using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateEvent : Event
    {
        protected AggregateEvent(Guid aggregateId, int aggregateVersion)
        {
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
        }

        protected AggregateEvent(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion)
            : base(id, dateTime)
        {
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
        }

        public Guid AggregateId { get; }

        public int AggregateVersion { get; }
    }
}
