namespace Microsoft.Extensions.DependencyInjection
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
