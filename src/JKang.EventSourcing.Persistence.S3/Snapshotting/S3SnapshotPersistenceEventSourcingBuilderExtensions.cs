using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence.S3.Snapshotting;
using JKang.EventSourcing.Snapshotting.Persistence;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class S3SnapshotPersistenceEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseS3SnapshotStore<TAggregate, TKey>(
            this IEventSourcingBuilder builder,
            Action<S3SnapshotStoreOptions> setupAction)
            where TAggregate : IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .ConfigureAggregate<TAggregate, TKey, S3SnapshotStoreOptions>(setupAction)
                .AddScoped<ISnapshotStore<TAggregate, TKey>, S3SnapshotStore<TAggregate, TKey>>()
                ;
            return builder;
        }
    }
}
