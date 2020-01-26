using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.FileSystem;
using JKang.EventSourcing.Serialization;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceFileSystemEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseTextFileEventStore<TAggregate, TAggregateKey>(
            this IEventSourcingBuilder builder,
            Action<TextFileEventStoreOptions> setupAction)
            where TAggregate : IAggregate<TAggregateKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .AddScoped<IEventStore<TAggregate, TAggregateKey>, TextFileEventStore<TAggregate, TAggregateKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TAggregateKey>, DefaultEventStoreInitializer<TAggregate, TAggregateKey>>()
                .Configure(setupAction)
                ;
            return builder;
        }
    }
}
