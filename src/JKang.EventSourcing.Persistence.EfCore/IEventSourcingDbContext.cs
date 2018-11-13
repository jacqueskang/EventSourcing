using Microsoft.EntityFrameworkCore;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public interface IEventSourcingDbContext
    {
        DbSet<EventEntity> Events { get; set; }
    }
}
