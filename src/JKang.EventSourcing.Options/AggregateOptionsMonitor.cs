using JKang.EventSourcing.Domain;
using Microsoft.Extensions.Options;

namespace JKang.EventSourcing.Options
{
    public class AggregateOptionsMonitor<TAggregate, TKey, TOptions>
        : IAggregateOptionsMonitor<TAggregate, TKey, TOptions>
        where TAggregate : IAggregate<TKey>
        where TOptions: class
    {
        private readonly IOptionsMonitor<TOptions> _monitor;

        public AggregateOptionsMonitor(IOptionsMonitor<TOptions> monitor)
        {
            _monitor = monitor;
        }

        public TOptions AggregateOptions => _monitor.Get(typeof(TAggregate).FullName);
    }
}
