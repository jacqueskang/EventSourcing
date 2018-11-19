using JKang.EventSourcing.Events;
using System.Collections.Generic;

namespace JKang.EventSourcing.Domain
{
    public interface IAggregateChangeset<TKey>
    {
        IEnumerable<IAggregateEvent<TKey>> Events { get; }
        void Commit();
    }
}