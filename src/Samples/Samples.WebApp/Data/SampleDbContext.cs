using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using Samples.Domain;
using System;

namespace Samples.WebApp.Data
{
    public class SampleDbContext : DbContext, IEventSourcingDbContext<GiftCard, Guid>
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        { }

        public DbSet<EventEntity<Guid>> GiftCardEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventEntityConfiguration<Guid>());
        }

        DbSet<EventEntity<Guid>> IEventSourcingDbContext<GiftCard, Guid>.GetDbSet()
        {
            return GiftCardEvents;
        }
    }
}
