using JKang.EventSourcing;
using JKang.EventSourcing.DependencyInjection;
using JKang.EventSourcing.Domain;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventSourcingServiceCollectionExtensions
    {
        public static IServiceCollection AddEventSourcing(this IServiceCollection services, Action<IEventSourcingBuilder> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services
                .AddSingleton(typeof(IAggregateOptionsMonitor<,,>), typeof(AggregateOptionsMonitor<,,>));

            IEventSourcingBuilder builder = new EventSourcingBuilder(services)
                .UseJsonEventSerializer()
                ;

            setupAction?.Invoke(builder);

            return services;
        }

        /// <summary>
        /// Configure options for an aggregate type
        /// </summary>
        public static IServiceCollection ConfigureAggregate<TAggregate, TAggregateKey, TAggregateOptions>(
            this IServiceCollection services,
            Action<TAggregateOptions> configureOptions)
            where TAggregate : IAggregate<TAggregateKey>
            where TAggregateOptions : class
            => services.Configure(typeof(TAggregate).FullName, configureOptions);
    }
}
