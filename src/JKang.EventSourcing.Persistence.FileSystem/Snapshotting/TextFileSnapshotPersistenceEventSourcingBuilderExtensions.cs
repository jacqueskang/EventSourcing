using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence.FileSystem.Snapshotting;
using JKang.EventSourcing.Snapshotting.Persistence;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TextFileSnapshotPersistenceEventSourcingBuilderExtensions
    {
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
                .AddScoped<ISnapshotStoreInitializer<TAggregate, TKey>, TextFileSnapshotStoreInitializer<TAggregate, TKey>>()
                .AddScoped<ISnapshotStore<TAggregate, TKey>, TextFileSnapshotStore<TAggregate, TKey>>()
                ;

            return builder;
        }
    }
}
