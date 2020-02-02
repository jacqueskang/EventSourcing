using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.CosmosDB;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CosmosDBEventPersistenceEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseCosmosDBEventStore<TAggregate, TKey>(
            this IEventSourcingBuilder builder,
            Action<CosmosDBEventStoreOptions> setupAction)
            where TAggregate : IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .ConfigureAggregate<TAggregate, TKey, CosmosDBEventStoreOptions>(setupAction)
                .AddScoped<IEventStore<TAggregate, TKey>, CosmosDBEventStore<TAggregate, TKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TKey>, CosmosDBEventStoreInitializer<TAggregate, TKey>>()
                ;

            return builder.TryUseDefaultSnapshotStore<TAggregate, TKey>();
        }
    }
}
