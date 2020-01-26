using JKang.EventSourcing.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStoreInitializer<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
    {
        Task EnsureCreatedAsync(CancellationToken cancellationToken = default);
    }
}
