using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using Samples.Domain;
using System;

namespace Samples.WebApp.Data
{
    public class SampleDbContext : DbContext, IEventDbContext<GiftCard, Guid>
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        { }

        public DbSet<EventEntity<Guid>> GiftCardEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfiguration(new EventEntityConfiguration<Guid>());

        DbSet<EventEntity<Guid>> IEventDbContext<GiftCard, Guid>.GetEventDbSet()
            => GiftCardEvents;
    }
}
