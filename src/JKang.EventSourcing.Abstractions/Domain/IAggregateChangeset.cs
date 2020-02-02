using JKang.EventSourcing.Events;
using JKang.EventSourcing.Snapshotting;
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