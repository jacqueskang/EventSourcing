namespace JKang.EventSourcing.Events
{
    public interface IAggregateEvent<TAggregateKey>
    {
        TAggregateKey AggregateId { get; }
        int AggregateVersion { get; }
    }
}
