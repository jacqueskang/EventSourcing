using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.FileSystem;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceFileSystemEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseTextFileEventStore<TEventSourcedAggregate>(
            this IEventSourcingBuilder builder,
            Action<TextFileEventStoreOptions> setupAction)
            where TEventSourcedAggregate : EventSourcedAggregate
        {
            builder.Services
                .AddScoped<IEventStore<TEventSourcedAggregate>, TextFileEventStore<TEventSourcedAggregate>>()
                .Configure(setupAction)
                ;
            return builder;
        }
    }
}
