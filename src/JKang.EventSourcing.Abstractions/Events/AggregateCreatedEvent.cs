using System;

namespace JKang.EventSourcing.Events
{
    public abstract class AggregateCreatedEvent : AggregateEvent
    {
        private const int _aggregateVersion = 1;

        protected AggregateCreatedEvent(Guid aggregateId)
            : base(aggregateId, aggregateVersion: _aggregateVersion)
        { }


        protected AggregateCreatedEvent(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion)
            : base(id, dateTime, aggregateId, aggregateVersion)
        {
            if (aggregateVersion != _aggregateVersion)
            {
                throw new InvalidOperationException($"Aggregate version is not {_aggregateVersion}");
            }
        }
    }
}
