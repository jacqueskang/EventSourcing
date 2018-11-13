using JKang.EventSourcing.Serialization;
using JKang.EventSourcing.Serialization.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SerializationJsonEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseJsonEventSerializer(
            this IEventSourcingBuilder builder)
        {
            builder.Services
                .AddScoped<ITextEventSerializer, JsonEventSerializer>()
                ;
            return builder;
        }
    }
}
