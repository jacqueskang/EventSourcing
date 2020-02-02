using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Options;
using JKang.EventSourcing.Snapshotting.Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.DynamoDB.Snapshotting
{
    public class DynamoDBSnapshotStoreInitializer<TAggregate, TKey>
        : ISnapshotStoreInitializer<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly DynamoDBSnapshotStoreOptions _options;
        private readonly IAmazonDynamoDB _dynamoDB;

        public DynamoDBSnapshotStoreInitializer(
            IAggregateOptionsMonitor<TAggregate, TKey, DynamoDBSnapshotStoreOptions> monitor,
            IAmazonDynamoDB dynamoDB)
        {
            if (monitor is null)
            {
                throw new ArgumentNullException(nameof(monitor));
            }

            _options = monitor.AggregateOptions;
            _dynamoDB = dynamoDB ?? throw new ArgumentNullException(nameof(dynamoDB));
        }

        public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            ListTablesResponse tables = await _dynamoDB.ListTablesAsync(cancellationToken).ConfigureAwait(false);
            if (tables.TableNames.Contains(_options.TableName))
            {
                return;
            }

            await _dynamoDB.CreateTableAsync(new CreateTableRequest
            {
                TableName = _options.TableName,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "aggregateId",
                        AttributeType = Defaults.IsNumericType<TKey>() ? "N" : "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "aggregateVersion",
                        AttributeType = "N"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "aggregateId",
                        KeyType = "HASH" //Partition key
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "aggregateVersion",
                        KeyType = "RANGE" //Sort key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                },
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
