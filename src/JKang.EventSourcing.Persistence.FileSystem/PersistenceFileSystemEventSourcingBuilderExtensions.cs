using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.FileSystem;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceFileSystemEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseTextFileEventStore(
            this IEventSourcingBuilder builder,
            Action<TextFileEventStoreOptions> setupAction)
        {
            builder.Services
                .AddScoped<IEventStore, TextFileEventStore>()
                .Configure(setupAction)
                ;
            return builder;
        }
    }
}
