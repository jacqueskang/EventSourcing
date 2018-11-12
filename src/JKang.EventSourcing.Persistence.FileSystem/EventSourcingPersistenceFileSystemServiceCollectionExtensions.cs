using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.FileSystem;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventSourcingPersistenceFileSystemServiceCollectionExtensions
    {
        public static IServiceCollection AddEventSourcingPersistenceFileSystem(this IServiceCollection services)
        {
            return services
                .AddScoped<IBinaryStore, FileSystemBinaryStore>()
                ;
        }
    }
}
