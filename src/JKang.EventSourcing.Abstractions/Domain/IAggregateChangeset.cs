using JKang.EventSourcing.Caching;
using JKang.EventSourcing.Events;
using System.Collections.Generic;

namespace JKang.EventSourcing.Domain
{
    public interface IAggregateChangeset<TKey>
    {
        IEnumerable<IAggregateEvent<TKey>> Events { get; }

        IEnumerable<IAggregateSnapshot<TKey>> Snapshots { get; }

        void Commit();
    }
}