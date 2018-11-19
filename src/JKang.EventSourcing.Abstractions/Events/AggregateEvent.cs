namespace JKang.EventSourcing.Events
{
    public abstract class AggregateEvent<TAggregateKey> : IAggregateEvent<TAggregateKey>
    {
        protected AggregateEvent(TAggregateKey aggregateId, int aggregateVersion)
        {
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
        }

        public TAggregateKey AggregateId { get; }
        public int AggregateVersion { get; }
    }
}
