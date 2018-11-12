using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Samples.WebApp.Data
{
    public class SampleDbContext : DbContext, IEventSourcingDbContext
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        { }

        public DbSet<EfEvent> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EfEventConfiguration());
        }
    }
}
