using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.FileSystem;
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
            builder.Services
                .AddScoped<IEventStore<TAggregate, TAggregateKey>, TextFileEventStore<TAggregate, TAggregateKey>>()
                .Configure(setupAction)
                ;
            return builder;
        }
    }
}
