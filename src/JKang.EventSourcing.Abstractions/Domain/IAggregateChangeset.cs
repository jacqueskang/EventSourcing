using JKang.EventSourcing.Events;
using System.Collections.Generic;

namespace JKang.EventSourcing.Domain
{
    public interface IAggregateChangeset
    {
        IEnumerable<IAggregateEvent> Events { get; }
        void Commit();
    }
}