using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DatabasePersistenceEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDbEventStore<TEventSourcingDbContext, TAggregate, TKey>(
            this IEventSourcingBuilder builder)
            where TEventSourcingDbContext : DbContext, IEventSourcingDbContext<TAggregate, TKey>
            where TAggregate : IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            builder.Services
                .AddScoped<IEventStore<TAggregate, TKey>, DatabaseEventStore<TEventSourcingDbContext, TAggregate, TKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TKey>, DatabaseEventStoreInitializer<TEventSourcingDbContext, TAggregate, TKey>>()
                ;

            return builder.TryUseDefaultSnapshotStore<TAggregate, TKey>();
        }
    }
}
