using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.EfCore;
using JKang.EventSourcing.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceEfCoreEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDbEventStore<TEventSourcingDbContext, TAggregate, TAggregateKey>(
            this IEventSourcingBuilder builder)
            where TEventSourcingDbContext : DbContext, IEventSourcingDbContext<TAggregate, TAggregateKey>
            where TAggregate : IAggregate<TAggregateKey>
        {
            builder.Services
                .AddScoped<IEventStore<TAggregate, TAggregateKey>, DatabaseEventStore<TEventSourcingDbContext, TAggregate, TAggregateKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TAggregateKey>, DefaultEventStoreInitializer<TAggregate, TAggregateKey>>()
                ;
            return builder;
        }
    }
}
