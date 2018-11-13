using Microsoft.Extensions.DependencyInjection;

namespace JKang.EventSourcing.DependencyInjection
{
    public class EventSourcingBuilder : IEventSourcingBuilder
    {
        public EventSourcingBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
