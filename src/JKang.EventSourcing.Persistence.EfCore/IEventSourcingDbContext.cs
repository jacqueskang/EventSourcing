using Microsoft.EntityFrameworkCore;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public interface IEventSourcingDbContext<TEventSourcedAggregate>
    {
        DbSet<EventEntity> GetDbSet();
    }
}
