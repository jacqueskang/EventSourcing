using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.DynamoDB;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceDynamoDBEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDynamoDBEventStore<TAggregate, TAggregateKey>(
            this IEventSourcingBuilder builder,
            Action<DynamoDBEventStoreOptions> setupAction)
            where TAggregate : IAggregate<TAggregateKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .ConfigureAggregate<TAggregate, TAggregateKey, DynamoDBEventStoreOptions>(setupAction)
                .AddScoped<IEventStore<TAggregate, TAggregateKey>, DynamoDBEventStore<TAggregate, TAggregateKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TAggregateKey>, DynamoDBEventStoreInitializer<TAggregate, TAggregateKey>>()
                ;

            return builder;
        }
    }
}
