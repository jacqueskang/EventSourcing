namespace JKang.EventSourcing.Caching
{
    public abstract class AggregateSnapshot<TAggregateKey> : IAggregateSnapshot<TAggregateKey>
    {
        protected AggregateSnapshot(TAggregateKey aggregateId, int aggregateVersion)
        {
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
        }

        public TAggregateKey AggregateId { get; }
        public int AggregateVersion { get; }
    }
}