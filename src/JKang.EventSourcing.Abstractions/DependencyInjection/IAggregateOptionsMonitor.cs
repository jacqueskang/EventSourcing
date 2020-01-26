using JKang.EventSourcing.Domain;

namespace JKang.EventSourcing.DependencyInjection
{
    public interface IAggregateOptionsMonitor<TAggregate, TKey, TOptions>
        where TAggregate: IAggregate<TKey>
        where TOptions: class
    {
        TOptions AggregateOptions { get; }
    }
}
