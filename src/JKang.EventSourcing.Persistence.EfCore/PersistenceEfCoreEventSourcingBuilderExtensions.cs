using JKang.EventSourcing.DependencyInjection;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceEfCoreEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDbEventStore<TEventSourcingDbContext, TAggregate>(
            this IEventSourcingBuilder builder)
            where TEventSourcingDbContext : DbContext, IEventSourcingDbContext<TAggregate>
            where TAggregate : IAggregate
        {
            builder.Services
                .AddScoped<IEventStore<TAggregate>, DatabaseEventStore<TEventSourcingDbContext, TAggregate>>()
                ;
            return builder;
        }
    }
}
