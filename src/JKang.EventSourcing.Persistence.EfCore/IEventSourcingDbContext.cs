using JKang.EventSourcing.Domain;
using Microsoft.EntityFrameworkCore;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public interface IEventSourcingDbContext<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        DbSet<EventEntity<TKey>> GetDbSet();
    }
}
