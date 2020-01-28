namespace JKang.EventSourcing.Events
{
    public interface IAggregateSnapshot<TAggregateKey>
        : IAggregateEvent<TAggregateKey>
    {
    }
}
