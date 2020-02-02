using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.FileSystem;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TextFileEventPersistenceEventSourcingBuilderExtensions
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
                .AddScoped<IEventStore<TAggregate, TKey>, TextFileEventStore<TAggregate, TKey>>();

            return builder.TryUseDefaultSnapshotStore<TAggregate, TKey>();
        }
    }
}
