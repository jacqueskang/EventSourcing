using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using Samples.Domain;

namespace Samples.WebApp.Data
{
    public class SampleDbContext : DbContext, IEventSourcingDbContext<Account>
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        { }

        public DbSet<EventEntity> AccountEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventEntityConfiguration());
        }

        DbSet<EventEntity> IEventSourcingDbContext<Account>.GetDbSet()
        {
            return AccountEvents;
        }
    }
}
