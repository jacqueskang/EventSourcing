using JKang.EventSourcing.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseEventStore<TEventStore>(this IEventSourcingBuilder builder)
            where TEventStore : class, IEventStore
        {
            builder.Services
                .AddScoped<IEventStore, TEventStore>()
                ;
            return builder;
        }

        public static IEventSourcingBuilder UseDefaultEventStore(this IEventSourcingBuilder builder)
        {
            return builder.UseEventStore<DefaultEventStore>();
        }

        public static IEventSourcingBuilder UserEventSerializer<TEventSerializer>(this IEventSourcingBuilder builder)
            where TEventSerializer: class, IEventSerializer
        {
            builder.Services
                .AddScoped<IEventSerializer, TEventSerializer>()
                ;
            return builder;
        }

        public static IEventSourcingBuilder UserDefaultEventSerializer(this IEventSourcingBuilder builder)
        {
            return builder.UserEventSerializer<DefaultEventSerializer>();
        }
    }
}
