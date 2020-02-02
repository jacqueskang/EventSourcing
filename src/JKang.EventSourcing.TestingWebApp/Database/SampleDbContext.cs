using JKang.EventSourcing.Persistence.EfCore;
using JKang.EventSourcing.Persistence.EfCore.Snapshotting;
using JKang.EventSourcing.TestingFixtures;
using Microsoft.EntityFrameworkCore;
using System;

namespace JKang.EventSourcing.TestingWebApp.Database
{
    public class SampleDbContext : DbContext,
        IEventDbContext<GiftCard, Guid>,
        ISnapshotDbContext<GiftCard, Guid>
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        { }

        public DbSet<EventEntity<Guid>> GiftCardEvents { get; set; }

        public DbSet<SnapshotEntity<Guid>> GiftCardSnapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventEntityConfiguration<Guid>());
            modelBuilder.ApplyConfiguration(new SnapshotEntityConfiguration<Guid>());
        }

        DbSet<EventEntity<Guid>> IEventDbContext<GiftCard, Guid>.GetEventDbSet() => GiftCardEvents;

        DbSet<SnapshotEntity<Guid>> ISnapshotDbContext<GiftCard, Guid>.GetSnapshotDbSet() => GiftCardSnapshots;
    }
}
