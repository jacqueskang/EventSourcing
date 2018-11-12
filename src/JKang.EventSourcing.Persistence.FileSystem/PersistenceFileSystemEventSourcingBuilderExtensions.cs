using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.FileSystem;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceFileSystemEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseFileSystemBinaryStore(
            this IEventSourcingBuilder builder,
            Action<FileSystemBinaryStoreOptions> setupAction)
        {
            builder.Services
                .AddScoped<IBinaryStore, FileSystemBinaryStore>()
                .Configure(setupAction)
                ;
            return builder;
        }
    }
}
