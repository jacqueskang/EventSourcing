using System;

namespace JKang.EventSourcing.Events
{
    public interface IAggregateEvent
    {
        Guid Id { get; }
        DateTime DateTime { get; }
        Guid AggregateId { get; }
        int AggregateVersion { get; }
    }
}
