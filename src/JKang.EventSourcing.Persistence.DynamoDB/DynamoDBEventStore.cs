﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Serialization.Json;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.DynamoDB
{
    public class DynamoDBEventStore<TAggregate, TAggregateKey> : IEventStore<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
    {
        private readonly IJsonObjectSerializer _serializer;
        private readonly Table _table;

        public DynamoDBEventStore(
            IJsonObjectSerializer serializer,
            IOptionsMonitor<DynamoDBEventStoreOptions> monitor,
            IAmazonDynamoDB client)
        {
            _serializer = serializer;
            DynamoDBEventStoreOptions options = monitor.Get(typeof(TAggregate).FullName);
            if (options.UseLocalDB)
            {
                client = options.CreateLocalDBClient();
            }
            _table = Table.LoadTable(client, options.TableName);
        }

        public async Task AddEventAsync(
            IAggregateEvent<TAggregateKey> @event,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string json = _serializer.Serialize(@event);
            var item = Document.FromJson(json);
            await _table.PutItemAsync(item, cancellationToken).ConfigureAwait(false);
        }

        private static T Convert<T>(DynamoDBEntry entry)
        {
            Type type = typeof(T);
            if (type == typeof(bool)) return (T)(object)entry.AsBoolean();
            if (type == typeof(byte)) return (T)(object)entry.AsByte();
            if (type == typeof(byte[])) return (T)(object)entry.AsByteArray();
            if (type == typeof(char)) return (T)(object)entry.AsChar();
            if (type == typeof(DateTime)) return (T)(object)entry.AsDateTime();
            if (type == typeof(decimal)) return (T)(object)entry.AsDecimal();
            if (type == typeof(double)) return (T)(object)entry.AsDouble();
            if (type == typeof(Guid)) return (T)(object)entry.AsGuid();
            if (type == typeof(int)) return (T)(object)entry.AsInt();
            if (type == typeof(long)) return (T)(object)entry.AsLong();
            if (type == typeof(MemoryStream)) return (T)(object)entry.AsMemoryStream();
            if (type == typeof(sbyte)) return (T)(object)entry.AsSByte();
            if (type == typeof(short)) return (T)(object)entry.AsShort();
            if (type == typeof(float)) return (T)(object)entry.AsSingle();
            if (type == typeof(string)) return (T)(object)entry.AsString();
            if (type == typeof(uint)) return (T)(object)entry.AsUInt();
            if (type == typeof(ulong)) return (T)(object)entry.AsULong();
            if (type == typeof(ushort)) return (T)(object)entry.AsUShort();
            throw new InvalidOperationException($"{type.FullName} is not supported as aggregate key in DynamoDB");
        }

        public async Task<TAggregateKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var scanFilter = new ScanFilter();
            //scanFilter.AddCondition("aggregateVersion", ScanOperator.Equal, 1);
            Search search = _table.Scan(scanFilter);

            var ids = new HashSet<TAggregateKey>();
            do
            {
                List<Document> documents = await search.GetNextSetAsync(cancellationToken);
                foreach (Document document in documents)
                {
                    DynamoDBEntry entry = document["aggregateId"];
                    TAggregateKey id = Convert<TAggregateKey>(entry);
                    ids.Add(id);
                }
            }
            while (!search.IsDone);

            return ids.ToArray();
        }

        public async Task<IAggregateEvent<TAggregateKey>[]> GetEventsAsync(
            TAggregateKey aggregateId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Search search = _table.Query(aggregateId as dynamic, new QueryFilter());

            var events = new List<IAggregateEvent<TAggregateKey>>();
            do
            {
                List<Document> documents = await search.GetNextSetAsync(cancellationToken).ConfigureAwait(false);
                foreach (Document document in documents)
                {
                    string json = document.ToJson();
                    IAggregateEvent<TAggregateKey> @event = _serializer.Deserialize<IAggregateEvent<TAggregateKey>>(json);
                    events.Add(@event);
                }
            } while (!search.IsDone);

            return events.OrderBy(x => x.AggregateVersion).ToArray();
        }
    }
}