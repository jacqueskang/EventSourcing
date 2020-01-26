using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.CosmosDB;
using JKang.EventSourcing.Persistence.DynamoDB;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
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

        public static IEventSourcingBuilder UseCosmosDBEventStore<TAggregate, TAggregateKey>(
            this IEventSourcingBuilder builder,
            Action<CosmosDBEventStoreOptions> setupAction)
            where TAggregate : IAggregate<TAggregateKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .ConfigureAggregate<TAggregate, TAggregateKey, CosmosDBEventStoreOptions>(setupAction)
                .AddScoped<IEventStore<TAggregate, TAggregateKey>, CosmosDBEventStore<TAggregate, TAggregateKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TAggregateKey>, CosmosDBEventStoreInitializer<TAggregate, TAggregateKey>>()
                ;

            return builder;
        }

    }
}
