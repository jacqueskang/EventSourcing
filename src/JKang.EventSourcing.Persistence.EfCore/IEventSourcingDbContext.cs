using Microsoft.EntityFrameworkCore;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public interface IEventSourcingDbContext
    {
        DbSet<EfEvent> Events { get; set; }
    }
}
