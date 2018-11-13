using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class EventEntityConfiguration : IEntityTypeConfiguration<EventEntity>
    {
        public void Configure(EntityTypeBuilder<EventEntity> builder)
        {
            builder.HasIndex(x => new
            {
                x.AggreagateType,
                x.AggregateId,
            });
        }
    }
}
