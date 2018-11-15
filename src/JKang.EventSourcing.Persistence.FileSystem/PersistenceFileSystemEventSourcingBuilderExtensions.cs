using JKang.EventSourcing.DependencyInjection;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.FileSystem;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceFileSystemEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseTextFileEventStore<TAggregate>(
            this IEventSourcingBuilder builder,
            Action<TextFileEventStoreOptions> setupAction)
            where TAggregate : IAggregate
        {
            builder.Services
                .AddScoped<IEventStore<TAggregate>, TextFileEventStore<TAggregate>>()
                .Configure(setupAction)
                ;
            return builder;
        }
    }
}
