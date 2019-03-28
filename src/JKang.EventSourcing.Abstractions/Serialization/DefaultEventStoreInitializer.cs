using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Serialization
{
    public class DefaultEventStoreInitializer<TAggregate, TAggregateKey> : IEventStoreInitializer<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
    {
        public Task EnsureCreatedAsync(CancellationToken cancellationToken = default(CancellationToken)) => Task.CompletedTask;
    }
}
