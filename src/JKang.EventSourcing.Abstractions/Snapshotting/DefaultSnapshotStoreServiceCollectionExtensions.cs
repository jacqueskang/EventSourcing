using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Snapshotting.Persistence;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DefaultSnapshotStoreServiceCollectionExtensions
    {
        public static IEventSourcingBuilder TryUseDefaultSnapshotStore<TAggregate, TKey>(this IEventSourcingBuilder builder)
            where TAggregate: IAggregate<TKey>
        {
            if (builder is null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            builder.Services
                .TryAddScoped<ISnapshotStore<TAggregate, TKey>, DefaultSnapshotStore<TAggregate, TKey>>();

            builder.Services
                .TryAddScoped<ISnapshotStoreInitializer<TAggregate, TKey>, DefaultSnapshotStoreInitializer<TAggregate, TKey>>();

            return builder;

        }
    }
}
