using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateEvent : Event
    {
        protected AggregateEvent(Guid aggregateId, int version)
        {
            AggregateId = aggregateId;
            Version = version;
        }

        protected AggregateEvent(Guid id, DateTime dateTime, Guid aggregateId, int version)
            : base(id, dateTime)
        {
            AggregateId = aggregateId;
            Version = version;
        }

        public Guid AggregateId { get; }

        public int Version { get; }
    }
}
