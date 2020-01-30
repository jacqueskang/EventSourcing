using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.FileSystem
{
    public class TextFileEventStore<TAggregate, TKey> : IEventStore<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly TextFileEventStoreOptions _options;

        public TextFileEventStore(
            IAggregateOptionsMonitor<TAggregate, TKey, TextFileEventStoreOptions> monitor)
        {
            if (monitor is null)
            {
                throw new ArgumentNullException(nameof(monitor));
            }

            _options = monitor.AggregateOptions;
        }

        public async Task AddEventAsync(IAggregateEvent<TKey> @event,
            CancellationToken cancellationToken = default)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            string serialized = JsonConvert.SerializeObject(@event, Standards.JsonSerializerSettings);
            string filePath = GetFilePath(@event.AggregateId);
            using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var sw = new StreamWriter(fs))
            {
                await sw.WriteLineAsync(serialized).ConfigureAwait(false);
            }
        }

        public Task<TKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var di = new DirectoryInfo(_options.Folder);
                if (!di.Exists)
                {
                    return Array.Empty<TKey>();
                }

                return di.GetFiles("*.events", SearchOption.TopDirectoryOnly)
                    .Select(x => x.Name)
                    .Select(x => Path.GetFileNameWithoutExtension(x))
                    .Select(x =>
                    {
                        MethodInfo mi = typeof(TKey).GetMethod("Parse", new Type[] { typeof(string) });
                        if (mi == null)
                        {
                            throw new InvalidOperationException($"Type '{typeof(TKey).Name}' must have a static method Parse(string)");
                        }
                        return (TKey)mi.Invoke(null, new object[] { x });
                    })
                    .ToArray();
            });
        }

        public async Task<IAggregateEvent<TKey>[]> GetEventsAsync(TKey aggregateId,
            int skip = 0,
            CancellationToken cancellationToken = default)
        {
            string filePath = GetFilePath(aggregateId);
            if (!File.Exists(filePath))
            {
                return Array.Empty<IAggregateEvent<TKey>>();
            }

            var events = new List<IAggregateEvent<TKey>>();
            using (FileStream fs = File.OpenRead(filePath))
            using (var sr = new StreamReader(fs))
            {
                string serialized = await sr.ReadLineAsync().ConfigureAwait(false);
                while (!string.IsNullOrEmpty(serialized))
                {
                    if (skip > 0)
                    {
                        skip--;
                    }
                    else
                    {
                        events.Add(JsonConvert.DeserializeObject<IAggregateEvent<TKey>>(serialized, Standards.JsonSerializerSettings));
                    }
                    serialized = await sr.ReadLineAsync().ConfigureAwait(false);
                }
            }
            return events.ToArray();
        }

        private string GetFilePath(TKey aggregateId)
        {
            return Path.Combine(_options.Folder, $"{aggregateId}.events");
        }
    }
}
