using System;

namespace JKang.EventSourcing.Events
{
    public interface IAggregateEvent<TAggregateKey>
    {
        // ID of domain aggregate
        TAggregateKey AggregateId { get; }

        // Version of domain aggregate after event occurred
        int AggregateVersion { get; }

        // Timestamp of event
        DateTime Timestamp { get; }
    }
}
