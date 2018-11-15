using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;

namespace JKang.EventSourcing.Domain
{
    public interface IAggregate
    {
        Guid Id { get; }
        int Version { get; }
        IEnumerable<IAggregateEvent> Events { get; }
        IAggregateChangeset GetChangeset();
    }
}