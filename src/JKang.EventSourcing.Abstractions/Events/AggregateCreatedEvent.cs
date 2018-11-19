namespace JKang.EventSourcing.Events
{
    public abstract class AggregateCreatedEvent<TAggregateKey> : AggregateEvent<TAggregateKey>
    {
        protected AggregateCreatedEvent(TAggregateKey aggregateId)
            : base(aggregateId, aggregateVersion: 1)
        { }
    }
}
