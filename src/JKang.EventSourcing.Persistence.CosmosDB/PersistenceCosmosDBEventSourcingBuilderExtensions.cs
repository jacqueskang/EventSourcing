using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.CosmosDB;
using JKang.EventSourcing.Persistence.DynamoDB;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceDynamoDBEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseCosmosDB(this IEventSourcingBuilder builder,
            string account, string key)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton(_ =>
            {
               return new CosmosClientBuilder(account, key)
                    .WithConnectionModeDirect()
                    .Build();
            });

            return builder;
        }

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
                .TryAddScoped<ISnapshotStore<TAggregate, TKey>, FakeSnapshotStore<TAggregate, TKey>>()
                ;

            return builder;
        }

    }
}
