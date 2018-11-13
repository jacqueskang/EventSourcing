using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Serialization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.FileSystem
{
    public class TextFileEventStore<TAggregate> : IEventStore<TAggregate>
        where TAggregate : Aggregate
    {
        private readonly IOptions<TextFileEventStoreOptions> _options;
        private readonly IObjectSerializer _eventSerializer;

        public TextFileEventStore(
            IOptions<TextFileEventStoreOptions> options,
            IObjectSerializer eventSerializer)
        {
            _options = options;
            _eventSerializer = eventSerializer;
        }

        public async Task AddEventAsync(AggregateEvent @event)
        {
            string serialized = _eventSerializer.Serialize(@event);
            string filePath = GetAggregateFilePath(@event.AggregateId, createFolderIfNotExist: true);
            using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var sw = new StreamWriter(fs))
            {
                if (fs.Position > 0)
                {
                    await sw.WriteAsync(_options.Value.EventSeparator);
                }
                await sw.WriteAsync(serialized);
            }
        }

        public Task<Guid[]> GetAggregateIdsAsync()
        {
            return Task.Run(() =>
            {
                string folder = GetAggregateFolder();
                var di = new DirectoryInfo(folder);
                if (!di.Exists)
                {
                    return new Guid[0];
                }

                return di.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
                    .Select(x => x.Name)
                    .Select(x => Path.GetFileNameWithoutExtension(x))
                    .Select(x => Guid.Parse(x))
                    .ToArray();
            });
        }

        public async Task<AggregateEvent[]> GetEventsAsync(Guid aggregateId)
        {
            string filePath = GetAggregateFilePath(aggregateId);
            if (!File.Exists(filePath))
            {
                return new AggregateEvent[0];
            }

            var events = new List<AggregateEvent>();
            string text;
            using (FileStream fs = File.OpenRead(filePath))
            using (var sr = new StreamReader(fs))
            {
                text = await sr.ReadToEndAsync();
            }

            return text.Split(new[] { _options.Value.EventSeparator }, StringSplitOptions.None)
                .Select(x => _eventSerializer.Deserialize<AggregateEvent>(x))
                .ToArray();
        }

        private string GetAggregateFilePath(Guid aggregateId, bool createFolderIfNotExist = false)
        {
            string folder = GetAggregateFolder(createFolderIfNotExist);
            return Path.Combine(folder, $"{aggregateId}.txt");
        }

        private string GetAggregateFolder(bool createIfNotExist = false)
        {
            if (createIfNotExist)
            {
                Directory.CreateDirectory(_options.Value.Folder);
            }
            return _options.Value.Folder;
        }
    }
}
