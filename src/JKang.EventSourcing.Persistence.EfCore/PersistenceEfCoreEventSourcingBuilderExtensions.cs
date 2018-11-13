using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceEfCoreEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDatabaseBinaryStore<TEventSourcingDbContext, TEventSourcedAggregate>(
            this IEventSourcingBuilder builder)
            where TEventSourcingDbContext : DbContext, IEventSourcingDbContext<TEventSourcedAggregate>
            where TEventSourcedAggregate : EventSourcedAggregate
        {
            builder.Services
                .AddScoped<IEventStore<TEventSourcedAggregate>, DatabaseEventStore<TEventSourcingDbContext, TEventSourcedAggregate>>()
                ;
            return builder;
        }
    }
}
