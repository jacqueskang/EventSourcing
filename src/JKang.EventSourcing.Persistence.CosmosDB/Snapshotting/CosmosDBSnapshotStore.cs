using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Options;
using JKang.EventSourcing.Snapshotting.Persistence;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.CosmosDB.Snapshotting
{
    public class CosmosDBSnapshotStore<TAggregate, TKey> : ISnapshotStore<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly Container _container;

        public CosmosDBSnapshotStore(
            CosmosClient client,
            IAggregateOptionsMonitor<TAggregate, TKey, CosmosDBSnapshotStoreOptions> monitor)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (monitor is null)
            {
                throw new ArgumentNullException(nameof(monitor));
            }

            CosmosDBSnapshotStoreOptions options = monitor.AggregateOptions;
            _container = client.GetContainer(options.DatabaseId, options.ContainerId);
        }

        public async Task AddSnapshotAsync(IAggregateSnapshot<TKey> snapshot,
            CancellationToken cancellationToken = default)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            await _container
                .CreateItemAsync(new
                {
                    id = Guid.NewGuid(),
                    data = snapshot
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IAggregateSnapshot<TKey>> FindLastSnapshotAsync(TKey aggregateId, int maxVersion,
            CancellationToken cancellationToken = default)
        {
            string query = $@"
SELECT TOP 1 VALUE c.data
FROM c
WHERE c.data.aggregateId = '{aggregateId}' AND c.data.aggregateVersion <= {maxVersion}
ORDER BY c.data.aggregateVersion DESC";

            FeedIterator<IAggregateSnapshot<TKey>> iterator = _container
                .GetItemQueryIterator<IAggregateSnapshot<TKey>>(new QueryDefinition(query));

            if (!iterator.HasMoreResults)
            {
                return null;
            }

            FeedResponse<IAggregateSnapshot<TKey>> response = await iterator
                .ReadNextAsync(cancellationToken)
                .ConfigureAwait(false)
                ;

            return response.FirstOrDefault();
        }
    }
}
