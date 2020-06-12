using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Options;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.CosmosDB
{
    public class CosmosDBEventStore<TAggregate, TKey> : IEventStore<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly Container _container;

        public CosmosDBEventStore(
            CosmosClient client,
            IAggregateOptionsMonitor<TAggregate, TKey, CosmosDBEventStoreOptions> monitor)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (monitor is null)
            {
                throw new ArgumentNullException(nameof(monitor));
            }

            CosmosDBEventStoreOptions options = monitor.AggregateOptions;
            _container = client.GetContainer(options.DatabaseId, options.ContainerId);
        }

        public async Task AddEventAsync(
            IAggregateEvent<TKey> e,
            CancellationToken cancellationToken = default)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            await _container
                .CreateItemAsync(new
                {
                    id = Guid.NewGuid(),
                    data = e
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<TKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default)
        {
            string query = "SELECT DISTINCT VALUE c.data.aggregateId FROM c";
            FeedIterator<TKey> iterator = _container.GetItemQueryIterator<TKey>(new QueryDefinition(query));

            var ids = new List<TKey>();

            while (iterator.HasMoreResults)
            {
                FeedResponse<TKey> results = await iterator
                    .ReadNextAsync(cancellationToken)
                    .ConfigureAwait(false);
                foreach (TKey id in results)
                {
                    ids.Add(id);
                }
            }

            return ids.ToArray();
        }

        public async Task<IAggregateEvent<TKey>[]> GetEventsAsync(
            TKey aggregateId,
            int minVersion,
            int maxVersion,
            CancellationToken cancellationToken = default)
        {
            string query = $@"
SELECT VALUE c.data
FROM c
WHERE c.data.aggregateId = '{aggregateId}' AND
      c.data.aggregateVersion >= {minVersion} AND
      c.data.aggregateVersion <= {maxVersion}
ORDER BY c.data.aggregateVersion";
            FeedIterator<IAggregateEvent<TKey>> iterator = _container
                .GetItemQueryIterator<IAggregateEvent<TKey>>(new QueryDefinition(query));
            var events = new List<IAggregateEvent<TKey>>();

            while (iterator.HasMoreResults)
            {
                FeedResponse<IAggregateEvent<TKey>> results = await iterator
                    .ReadNextAsync(cancellationToken)
                    .ConfigureAwait(false);
                foreach (IAggregateEvent<TKey> e in results)
                {
                    events.Add(e);
                }
            }

            return events.ToArray();
        }
    }
}
