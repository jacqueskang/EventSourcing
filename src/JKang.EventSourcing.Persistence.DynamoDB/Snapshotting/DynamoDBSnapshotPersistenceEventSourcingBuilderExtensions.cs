using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence.DynamoDB.Snapshotting;
using JKang.EventSourcing.Snapshotting.Persistence;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DynamoDBSnapshotPersistenceEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDynamoDBSnapshotStore<TAggregate, TKey>(
            this IEventSourcingBuilder builder,
            Action<DynamoDBSnapshotStoreOptions> setupAction)
            where TAggregate : IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .ConfigureAggregate<TAggregate, TKey, DynamoDBSnapshotStoreOptions>(setupAction)
                .AddScoped<ISnapshotStore<TAggregate, TKey>, DynamoDBSnapshotStore<TAggregate, TKey>>()
                .AddScoped<ISnapshotStoreInitializer<TAggregate, TKey>, DynamoDBSnapshotStoreInitializer<TAggregate, TKey>>()
                ;
            return builder;
        }
    }
}
