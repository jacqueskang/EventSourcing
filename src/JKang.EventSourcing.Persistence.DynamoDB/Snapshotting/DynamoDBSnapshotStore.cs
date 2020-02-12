using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Options;
using JKang.EventSourcing.Snapshotting.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.DynamoDB.Snapshotting
{
    public class DynamoDBSnapshotStore<TAggregate, TKey> : ISnapshotStore<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly Table _table;

        public DynamoDBSnapshotStore(
            IAggregateOptionsMonitor<TAggregate, TKey, DynamoDBSnapshotStoreOptions> monitor,
            IAmazonDynamoDB client)
        {
            if (monitor is null)
            {
                throw new ArgumentNullException(nameof(monitor));
            }

            _table = Table.LoadTable(client, monitor.AggregateOptions.TableName);
        }

        public async Task AddSnapshotAsync(IAggregateSnapshot<TKey> snapshot,
            CancellationToken cancellationToken = default)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            string json = JsonConvert.SerializeObject(snapshot, Defaults.JsonSerializerSettings);
            var item = Document.FromJson(json);
            await _table.PutItemAsync(item, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IAggregateSnapshot<TKey>> FindLastSnapshotAsync(TKey aggregateId, int maxVersion,
            CancellationToken cancellationToken = default)
        {

            var filter = new QueryFilter("aggregateId", QueryOperator.Equal, aggregateId as dynamic);
            filter.AddCondition("aggregateVersion", QueryOperator.LessThanOrEqual, maxVersion);
            Search search = _table.Query(new QueryOperationConfig
            {
                Filter = filter,
                Limit = 1,
                BackwardSearch = true,
            });

            List<Document> documents = await search.GetNextSetAsync(cancellationToken).ConfigureAwait(false);
            if (documents.Count == 0)
            {
                return null;
            }

            string json = documents.First().ToJson();
            IAggregateSnapshot<TKey> snapshot = JsonConvert.DeserializeObject<IAggregateSnapshot<TKey>>(json, Defaults.JsonSerializerSettings);
            return snapshot;
        }
    }
}
