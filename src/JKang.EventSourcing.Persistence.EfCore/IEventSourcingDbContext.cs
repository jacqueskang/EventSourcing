using JKang.EventSourcing.Domain;
using Microsoft.EntityFrameworkCore;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public interface IEventSourcingDbContext<TAggregate>
        where TAggregate : Aggregate
    {
        DbSet<EventEntity> GetDbSet();
    }
}
