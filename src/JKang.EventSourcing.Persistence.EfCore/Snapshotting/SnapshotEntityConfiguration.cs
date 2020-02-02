using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JKang.EventSourcing.Persistence.EfCore.Snapshotting
{
    public class SnapshotEntityConfiguration<TKey> : IEntityTypeConfiguration<SnapshotEntity<TKey>>
    {
        public void Configure(EntityTypeBuilder<SnapshotEntity<TKey>> builder)
        {
            if (builder is null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            builder.HasKey(x => new
            {
                x.AggregateId,
                x.AggregateVersion,
            });
        }
    }
}
