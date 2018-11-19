using JKang.EventSourcing.Domain;
using Microsoft.EntityFrameworkCore;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public interface IEventSourcingDbContext<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
    {
        DbSet<EventEntity<TAggregateKey>> GetDbSet();
    }
}
