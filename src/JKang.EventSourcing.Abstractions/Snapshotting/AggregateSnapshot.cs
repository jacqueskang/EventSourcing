using JKang.EventSourcing.Domain;

namespace JKang.EventSourcing.Snapshotting
{
    public abstract class AggregateSnapshot<TKey> : IAggregateSnapshot<TKey>
    {
        protected AggregateSnapshot(TKey aggregateId, int aggregateVersion)
        {
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
        }

        public TKey AggregateId { get; }
        public int AggregateVersion { get; }
    }
}