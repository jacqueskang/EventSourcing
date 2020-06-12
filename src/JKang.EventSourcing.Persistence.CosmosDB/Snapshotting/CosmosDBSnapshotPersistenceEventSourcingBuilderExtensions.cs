using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence.CosmosDB.Snapshotting;
using JKang.EventSourcing.Snapshotting.Persistence;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CosmosDBSnapshotPersistenceEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseCosmosDBSnapshotStore<TAggregate, TKey>(
            this IEventSourcingBuilder builder,
            Action<CosmosDBSnapshotStoreOptions> setupAction)
            where TAggregate : IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .ConfigureAggregate<TAggregate, TKey, CosmosDBSnapshotStoreOptions>(setupAction)
                .AddScoped<ISnapshotStore<TAggregate, TKey>, CosmosDBSnapshotStore<TAggregate, TKey>>()
                .AddScoped<ISnapshotStoreInitializer<TAggregate, TKey>, CosmosDBSnapshotStoreInitializer<TAggregate, TKey>>()
                ;
            return builder;
        }
    }
}
