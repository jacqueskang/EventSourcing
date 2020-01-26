using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Serialization;
using Microsoft.Extensions.Options;
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
        private readonly TextFileEventStoreOptions _options;
        private readonly IObjectSerializer _eventSerializer;

        public TextFileEventStore(
            IOptionsMonitor<TextFileEventStoreOptions> options,
            IObjectSerializer eventSerializer)
        {
            _options = options.CurrentValue;
            _eventSerializer = eventSerializer;
        }

        public async Task AddEventAsync(IAggregateEvent<TAggregateKey> @event,
            CancellationToken cancellationToken = default)
        {
            string serialized = _eventSerializer.Serialize(@event);
            string filePath = GetAggregateFilePath(@event.AggregateId, createFolderIfNotExist: true);
            using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var sw = new StreamWriter(fs))
            {
                if (fs.Position > 0)
                {
                    await sw.WriteAsync(_options.EventSeparator);
                }
                await sw.WriteAsync(serialized);
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
                    return new TAggregateKey[0];
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
                return new IAggregateEvent<TAggregateKey>[0];
            }

            var events = new List<IAggregateEvent<TAggregateKey>>();
            string text;
            using (FileStream fs = File.OpenRead(filePath))
            using (var sr = new StreamReader(fs))
            {
                text = await sr.ReadToEndAsync();
            }

            return text.Split(new[] { _options.EventSeparator }, StringSplitOptions.None)
                .Select(x => _eventSerializer.Deserialize<IAggregateEvent<TAggregateKey>>(x))
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
