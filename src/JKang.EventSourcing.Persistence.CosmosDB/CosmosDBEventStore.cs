using JKang.EventSourcing.DependencyInjection;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Persistence.DynamoDB;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.CosmosDB
{
    public class CosmosDBEventStore<TAggregate, TAggregateKey> : IEventStore<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
    {
        private readonly Container _container;

        public CosmosDBEventStore(
            CosmosClient client,
            IAggregateOptionsMonitor<TAggregate, TAggregateKey, CosmosDBEventStoreOptions> monitor)
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
            IAggregateEvent<TAggregateKey> e,
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

        public async Task<TAggregateKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default)
        {
            string query = "SELECT DISTINCT VALUE c.data.aggregateId FROM c";
            FeedIterator<TAggregateKey> iterator = _container.GetItemQueryIterator<TAggregateKey>(new QueryDefinition(query));

            var ids = new List<TAggregateKey>();

            while (iterator.HasMoreResults)
            {
                FeedResponse<TAggregateKey> results = await iterator.ReadNextAsync().ConfigureAwait(false);
                foreach (TAggregateKey id in results)
                {
                    ids.Add(id);
                }
            }

            return ids.ToArray();
        }

        public async Task<IAggregateEvent<TAggregateKey>[]> GetEventsAsync(
            TAggregateKey aggregateId,
            CancellationToken cancellationToken = default)
        {
            string query = $"SELECT VALUE c.data FROM c WHERE c.data.aggregateId = '{aggregateId}' ORDER BY c.data.aggregateVersion";
            FeedIterator<IAggregateEvent<TAggregateKey>> iterator = _container.GetItemQueryIterator<IAggregateEvent<TAggregateKey>>(new QueryDefinition(query));
            var events = new List<IAggregateEvent<TAggregateKey>>();

            while (iterator.HasMoreResults)
            {
                FeedResponse<IAggregateEvent<TAggregateKey>> results = await iterator.ReadNextAsync().ConfigureAwait(false);
                foreach (IAggregateEvent<TAggregateKey> e in results)
                {
                    events.Add(e);
                }
            }

            return events.ToArray();
        }
    }
}
