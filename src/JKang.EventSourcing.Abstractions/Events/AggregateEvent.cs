using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateEvent : IEvent
    {
        /// <summary>
        /// Constructor to create a new event
        /// </summary>
        protected AggregateEvent(Guid aggregateId, int aggregateVersion)
            : this(Guid.NewGuid(), DateTime.UtcNow, aggregateId, aggregateVersion)
        { }

        /// <summary>
        /// Constructor to create an existing event (e.g. for deserialization)
        /// </summary>
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
