using JKang.EventSourcing.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventSourcingPersistenceServiceCollectionExtensions
    {
        public static IServiceCollection AddEventSourcingPersistence(this IServiceCollection services)
        {
            return services
                .AddScoped<IEventStore, DefaultEventStore>()
                .AddScoped<IEventSerializer, DefaultEventSerializer>()
                ;
        }
    }
}
