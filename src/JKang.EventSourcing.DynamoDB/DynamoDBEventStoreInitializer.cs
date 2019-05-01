using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using JKang.EventSourcing.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.DynamoDB
{
    public class DynamoDBEventStoreInitializer<TAggregate, TAggregateKey>
        : IEventStoreInitializer<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
    {
        private readonly DynamoDBEventStoreOptions _options;
        private readonly IAmazonDynamoDB _dynamoDB;

        public DynamoDBEventStoreInitializer(
            IOptionsMonitor<DynamoDBEventStoreOptions> monitor,
            IAmazonDynamoDB dynamoDB)
        {
            _options = monitor.Get(typeof(TAggregate).FullName);
            _dynamoDB = _options.UseLocalDB
                ? _options.CreateLocalDBClient()
                : dynamoDB;
        } 

        private static bool IsNumericType()
        {
            switch (Type.GetTypeCode(typeof(TAggregateKey)))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default(CancellationToken))
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
                        AttributeType = IsNumericType() ? "N" : "S"
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
            }, cancellationToken)
            .ConfigureAwait(false);
        }
    }
}
