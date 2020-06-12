using JKang.EventSourcing.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventStoreInitializer<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        Task EnsureCreatedAsync(CancellationToken cancellationToken = default);
    }
}
