using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.DynamoDB;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DynamoDBEventPersistenceEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDynamoDBEventStore<TAggregate, TKey>(
            this IEventSourcingBuilder builder,
            Action<DynamoDBEventStoreOptions> setupAction)
            where TAggregate : IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .ConfigureAggregate<TAggregate, TKey, DynamoDBEventStoreOptions>(setupAction)
                .AddScoped<IEventStore<TAggregate, TKey>, DynamoDBEventStore<TAggregate, TKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TKey>, DynamoDBEventStoreInitializer<TAggregate, TKey>>()
                ;

            return builder.TryUseDefaultSnapshotStore<TAggregate, TKey>();
        }
    }
}
