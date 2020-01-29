using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.FileSystem;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceFileSystemEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseTextFileEventStore<TAggregate, TKey>(
            this IEventSourcingBuilder builder,
            Action<TextFileEventStoreOptions> setupAction)
            where TAggregate : IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .ConfigureAggregate<TAggregate, TKey, TextFileEventStoreOptions>(setupAction)
                .AddScoped<IEventStore<TAggregate, TKey>, TextFileEventStore<TAggregate, TKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TKey>, TextFileEventStoreInitializer<TAggregate, TKey>>()
                ;
            return builder;
        }

        public static IEventSourcingBuilder UseTextFileSnapshotStore<TAggregate, TKey>(
            this IEventSourcingBuilder builder,
            Action<TextFileSnapshotStoreOptions> setupAction)
            where TAggregate : IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .ConfigureAggregate<TAggregate, TKey, TextFileSnapshotStoreOptions>(setupAction)
                .AddScoped<ISnapshotStore<TAggregate, TKey>, TextFileSnapshotStore<TAggregate, TKey>>()
                ;
            return builder;
        }
    }
}
