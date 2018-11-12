using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceEfCoreEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDatabaseBinaryStore<TContext>(
            this IEventSourcingBuilder builder)
            where TContext : DbContext, IEventSourcingDbContext
        {
            builder.Services
                .AddScoped<IBinaryStore, DatabaseBinaryStore<TContext>>()
                ;
            return builder;
        }
    }
}
