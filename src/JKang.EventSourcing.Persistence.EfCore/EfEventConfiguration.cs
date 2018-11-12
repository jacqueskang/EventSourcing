using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class EfEventConfiguration : IEntityTypeConfiguration<EfEvent>
    {
        public void Configure(EntityTypeBuilder<EfEvent> builder)
        {
            builder.HasKey(x => new
            {
                x.Store,
                x.Container,
                x.Key
            });
        }
    }
}
