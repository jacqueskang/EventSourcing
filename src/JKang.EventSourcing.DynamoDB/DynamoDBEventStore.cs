using Amazon.DynamoDBv2;
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
    internal class DynamoDBEventStore<TAggregate, TAggregateKey> : IEventStore<TAggregate, TAggregateKey>
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
            Document re = await _table.PutItemAsync(item, cancellationToken);
        }

        private static T Convert<T>(Primitive primitive)
        {
            Type type = typeof(T);
            if (type == typeof(bool)) return (T)(object)primitive.AsBoolean();
            if (type == typeof(byte)) return (T)(object)primitive.AsByte();
            if (type == typeof(byte[])) return (T)(object)primitive.AsByteArray();
            if (type == typeof(char)) return (T)(object)primitive.AsChar();
            if (type == typeof(DateTime)) return (T)(object)primitive.AsDateTime();
            if (type == typeof(decimal)) return (T)(object)primitive.AsDecimal();
            if (type == typeof(double)) return (T)(object)primitive.AsDouble();
            if (type == typeof(Guid)) return (T)(object)primitive.AsGuid();
            if (type == typeof(int)) return (T)(object)primitive.AsInt();
            if (type == typeof(long)) return (T)(object)primitive.AsLong();
            if (type == typeof(MemoryStream)) return (T)(object)primitive.AsMemoryStream();
            if (type == typeof(sbyte)) return (T)(object)primitive.AsSByte();
            if (type == typeof(short)) return (T)(object)primitive.AsShort();
            if (type == typeof(float)) return (T)(object)primitive.AsSingle();
            if (type == typeof(string)) return (T)(object)primitive.AsString();
            if (type == typeof(uint)) return (T)(object)primitive.AsUInt();
            if (type == typeof(ulong)) return (T)(object)primitive.AsULong();
            if (type == typeof(ushort)) return (T)(object)primitive.AsUShort();
            throw new InvalidOperationException($"{type.FullName} is not supported as aggregate key in DynamoDB");
        }

        public Task<TAggregateKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(_table.HashKeys
                .Select(x => new Primitive(x))
                .Select(x => Convert<TAggregateKey>(x))
                .ToArray());
        }

        public async Task<IAggregateEvent<TAggregateKey>[]> GetEventsAsync(
            TAggregateKey aggregateId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Search search = _table.Query(aggregateId as dynamic, new QueryFilter());

            var events = new List<IAggregateEvent<TAggregateKey>>();
            do
            {
                List<Document> documents = await search.GetNextSetAsync(cancellationToken);
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