using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class EventEntityConfiguration<TAggregateKey> : IEntityTypeConfiguration<EventEntity<TAggregateKey>>
    {
        public void Configure(EntityTypeBuilder<EventEntity<TAggregateKey>> builder)
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
