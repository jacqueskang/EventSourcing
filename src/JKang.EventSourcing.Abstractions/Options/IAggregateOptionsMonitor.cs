using JKang.EventSourcing.Domain;

namespace JKang.EventSourcing.Options
{
    public interface IAggregateOptionsMonitor<TAggregate, TKey, TOptions>
        where TAggregate: IAggregate<TKey>
        where TOptions: class
    {
        TOptions AggregateOptions { get; }
    }
}
