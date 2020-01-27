using JKang.EventSourcing.DependencyInjection;
using JKang.EventSourcing.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventSourcingServiceCollectionExtensions
    {
        public static IServiceCollection AddEventSourcing(
            this IServiceCollection services,
            Action<IEventSourcingBuilder> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services
                .AddSingleton(typeof(IAggregateOptionsMonitor<,,>), typeof(AggregateOptionsMonitor<,,>))
                ;

            IEventSourcingBuilder builder = new EventSourcingBuilder(services);

            setupAction?.Invoke(builder);

            return services;
        }
    }
}
