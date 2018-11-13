using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceEfCoreEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDatabaseBinaryStore<TEventSourcingDbContext>(
            this IEventSourcingBuilder builder)
            where TEventSourcingDbContext : DbContext, IEventSourcingDbContext
        {
            builder.Services
                .AddScoped<IEventStore, DatabaseEventStore<TEventSourcingDbContext>>()
                ;
            return builder;
        }
    }
}
