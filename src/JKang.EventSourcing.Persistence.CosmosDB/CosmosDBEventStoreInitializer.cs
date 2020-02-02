using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Options;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.CosmosDB
{
    public class CosmosDBEventStoreInitializer<TAggregate, TKey>
        : IEventStoreInitializer<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly CosmosDBEventStoreOptions _options;
        private readonly CosmosClient _client;

        public CosmosDBEventStoreInitializer(
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

            _client = client;
            _options = monitor.AggregateOptions;
        }

        public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            Database database = await _client
                .CreateDatabaseIfNotExistsAsync(_options.DatabaseId, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            ContainerResponse containerResponse = await database.CreateContainerIfNotExistsAsync(_options.ContainerId,
                partitionKeyPath: "/data/aggregateId",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            ContainerProperties properties = containerResponse.Resource;

            const string indexPath = "/data/aggregateVersion/?";
            if (!properties.IndexingPolicy.IncludedPaths.Any(x => x.Path == indexPath))
            {
                containerResponse.Resource.IndexingPolicy.IncludedPaths.Add(new IncludedPath
                {
                    Path = indexPath
                });
                await containerResponse.Container
                    .ReplaceContainerAsync(containerResponse.Resource, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
