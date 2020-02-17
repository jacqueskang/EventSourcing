using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.DynamoDB
{
    public class DynamoDBEventStore<TAggregate, TKey> : IEventStore<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly Table _table;

        public DynamoDBEventStore(
            IAggregateOptionsMonitor<TAggregate, TKey, DynamoDBEventStoreOptions> monitor,
            IAmazonDynamoDB client)
        {
            if (monitor is null)
            {
                throw new ArgumentNullException(nameof(monitor));
            }

            _table = Table.LoadTable(client, monitor.AggregateOptions.TableName);
        }

        public async Task AddEventAsync(
            IAggregateEvent<TKey> @event,
            CancellationToken cancellationToken = default)
        {
            string json = JsonConvert.SerializeObject(@event, Defaults.JsonSerializerSettings);
            var item = Document.FromJson(json);
            await _table.PutItemAsync(item, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default)
        {
            var scanFilter = new ScanFilter();
            //scanFilter.AddCondition("aggregateVersion", ScanOperator.Equal, 1);
            Search search = _table.Scan(scanFilter);

            var ids = new HashSet<TKey>();
            do
            {
                List<Document> documents = await search.GetNextSetAsync(cancellationToken).ConfigureAwait(false);
                foreach (Document document in documents)
                {
                    DynamoDBEntry entry = document["aggregateId"];
                    TKey id = Defaults.Convert<TKey>(entry);
                    ids.Add(id);
                }
            }
            while (!search.IsDone);

            return ids.ToArray();
        }

        public async Task<IAggregateEvent<TKey>[]> GetEventsAsync(
            TKey aggregateId,
            int minVersion,
            int maxVersion,
            CancellationToken cancellationToken = default)
        {
            var filter = new QueryFilter("aggregateVersion", QueryOperator.Between, minVersion, maxVersion);
            Search search = this._table.Query(aggregateId as dynamic, filter);

            var events = new List<IAggregateEvent<TKey>>();
            do
            {
                List<Document> documents = await search.GetNextSetAsync(cancellationToken).ConfigureAwait(false);
                foreach (Document document in documents)
                {
                    string json = document.ToJson();
                    IAggregateEvent<TKey> @event = JsonConvert.DeserializeObject<IAggregateEvent<TKey>>(json, Defaults.JsonSerializerSettings);
                    events.Add(@event);
                }
            } while (!search.IsDone);

            return events.OrderBy(x => x.AggregateVersion).ToArray();
        }
    }
}