using JKang.EventSourcing.Events;
using System.Collections.Generic;

namespace JKang.EventSourcing.Domain
{
    public interface IAggregateChangeset<TKey>
    {
        IEnumerable<IAggregateEvent<TKey>> Events { get; }

        IAggregateSnapshot<TKey> Snapshot { get; }

        void Commit();
    }
}