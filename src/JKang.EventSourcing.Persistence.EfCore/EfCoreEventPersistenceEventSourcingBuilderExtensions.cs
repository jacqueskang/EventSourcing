using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EfCoreEventPersistenceEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseEfCoreEventStore<TEventDbContext, TAggregate, TKey>(
            this IEventSourcingBuilder builder)
            where TEventDbContext : DbContext, IEventDbContext<TAggregate, TKey>
            where TAggregate : IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            builder.Services
                .AddScoped<IEventStore<TAggregate, TKey>, EfCoreEventStore<TEventDbContext, TAggregate, TKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TKey>, EfCoreEventStoreInitializer<TEventDbContext, TAggregate, TKey>>()
                ;

            return builder.TryUseDefaultSnapshotStore<TAggregate, TKey>();
        }
    }
}
