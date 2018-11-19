using System;

namespace JKang.EventSourcing.Events
{
    public interface IAggregateEvent<TAggregateKey>
    {
        Guid Id { get; }
        DateTime DateTime { get; }
        TAggregateKey AggregateId { get; }
        int AggregateVersion { get; }
    }
}
