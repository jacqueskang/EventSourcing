using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence.EfCore.Snapshotting;
using JKang.EventSourcing.Snapshotting.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EfCoreSnapshotPersistenceEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseEfCoreSnapshotStore<TSnapshotDbContext, TAggregate, TKey>(
            this IEventSourcingBuilder builder)
            where TSnapshotDbContext : DbContext, ISnapshotDbContext<TAggregate, TKey>
            where TAggregate : IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            builder.Services
                .AddScoped<ISnapshotStore<TAggregate, TKey>, EfCoreSnapshotStore<TSnapshotDbContext, TAggregate, TKey>>()
                .AddScoped<ISnapshotStoreInitializer<TAggregate, TKey>, EfCoreSnapshotStoreInitializer<TSnapshotDbContext, TAggregate, TKey>>()
                ;

            return builder;
        }
    }
}
