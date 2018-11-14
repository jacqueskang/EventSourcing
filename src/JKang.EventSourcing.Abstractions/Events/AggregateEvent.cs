using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateEvent : IEvent
    {
        protected AggregateEvent(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion)
        {
            Id = id;
            DateTime = dateTime;
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
        }

        public Guid Id { get; }
        public DateTime DateTime { get; }
        public Guid AggregateId { get; }
        public int AggregateVersion { get; }
    }
}
