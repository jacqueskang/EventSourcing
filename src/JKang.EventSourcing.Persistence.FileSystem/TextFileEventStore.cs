using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            Converters = new[] { new StringEnumConverter() },
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
        };
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

            string serialized = JsonConvert.SerializeObject(@event, _jsonSerializerSettings);
            string filePath = GetAggregateFilePath(@event.AggregateId);
            using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var sw = new StreamWriter(fs))
            {
                if (fs.Position > 0)
                {
                    await sw.WriteAsync(_options.EventSeparator).ConfigureAwait(false);
                }
                await sw.WriteAsync(serialized).ConfigureAwait(false);
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

                return di.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
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
            CancellationToken cancellationToken = default)
        {
            string filePath = GetAggregateFilePath(aggregateId);
            if (!File.Exists(filePath))
            {
                return Array.Empty<IAggregateEvent<TKey>>();
            }

            var events = new List<IAggregateEvent<TKey>>();
            string text;
            using (FileStream fs = File.OpenRead(filePath))
            using (var sr = new StreamReader(fs))
            {
                text = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            return text.Split(new[] { _options.EventSeparator }, StringSplitOptions.None)
                .Select(x => JsonConvert.DeserializeObject<IAggregateEvent<TKey>>(x, _jsonSerializerSettings))
                .ToArray();
        }

        private string GetAggregateFilePath(TKey aggregateId)
        {
            return Path.Combine(_options.Folder, $"{aggregateId}.txt");
        }
    }
}
