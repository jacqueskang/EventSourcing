using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using Microsoft.Extensions.Options;
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
    public class TextFileEventStore<TAggregate, TAggregateKey> : IEventStore<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
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
            IOptionsMonitor<TextFileEventStoreOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.CurrentValue;
        }

        public async Task AddEventAsync(IAggregateEvent<TAggregateKey> @event,
            CancellationToken cancellationToken = default)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            string serialized = JsonConvert.SerializeObject(@event, _jsonSerializerSettings);
            string filePath = GetAggregateFilePath(@event.AggregateId, createFolderIfNotExist: true);
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

        public Task<TAggregateKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                string folder = GetAggregateFolder();
                var di = new DirectoryInfo(folder);
                if (!di.Exists)
                {
                    return Array.Empty<TAggregateKey>();
                }

                return di.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
                    .Select(x => x.Name)
                    .Select(x => Path.GetFileNameWithoutExtension(x))
                    .Select(x =>
                    {
                        MethodInfo mi = typeof(TAggregateKey).GetMethod("Parse", new Type[] { typeof(string) });
                        if (mi == null)
                        {
                            throw new InvalidOperationException($"Type '{typeof(TAggregateKey).Name}' must have a static method Parse(string)");
                        }
                        return (TAggregateKey)mi.Invoke(null, new object[] { x });
                    })
                    .ToArray();
            });
        }

        public async Task<IAggregateEvent<TAggregateKey>[]> GetEventsAsync(TAggregateKey aggregateId,
            CancellationToken cancellationToken = default)
        {
            string filePath = GetAggregateFilePath(aggregateId);
            if (!File.Exists(filePath))
            {
                return Array.Empty<IAggregateEvent<TAggregateKey>>();
            }

            var events = new List<IAggregateEvent<TAggregateKey>>();
            string text;
            using (FileStream fs = File.OpenRead(filePath))
            using (var sr = new StreamReader(fs))
            {
                text = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            return text.Split(new[] { _options.EventSeparator }, StringSplitOptions.None)
                .Select(x => JsonConvert.DeserializeObject<IAggregateEvent<TAggregateKey>>(x, _jsonSerializerSettings))
                .ToArray();
        }

        private string GetAggregateFilePath(TAggregateKey aggregateId, bool createFolderIfNotExist = false)
        {
            string folder = GetAggregateFolder(createFolderIfNotExist);
            return Path.Combine(folder, $"{aggregateId}.txt");
        }

        private string GetAggregateFolder(bool createIfNotExist = false)
        {
            if (createIfNotExist)
            {
                Directory.CreateDirectory(_options.Folder);
            }
            return _options.Folder;
        }
    }
}
