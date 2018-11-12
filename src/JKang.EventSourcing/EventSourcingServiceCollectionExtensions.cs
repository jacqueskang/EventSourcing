using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventSourcingServiceCollectionExtensions
    {
        public static IEventSourcingBuilder AddEventSourcing(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return new EventSourcingBuilder(services)
                .UseDefaultEventStore()
                .UserDefaultEventSerializer();
        }
    }
}
