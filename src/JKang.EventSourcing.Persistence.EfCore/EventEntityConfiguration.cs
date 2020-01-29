using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class EventEntityConfiguration<TKey> : IEntityTypeConfiguration<EventEntity<TKey>>
    {
        public void Configure(EntityTypeBuilder<EventEntity<TKey>> builder)
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
