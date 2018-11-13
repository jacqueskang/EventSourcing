using JKang.EventSourcing.Events;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.FileSystem
{
    public class TextFileEventStore : IEventStore
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            Converters = new[] { new StringEnumConverter() },
        };

        private readonly IOptions<TextFileEventStoreOptions> _options;

        public TextFileEventStore(IOptions<TextFileEventStoreOptions> options)
        {
            _options = options;
        }

        public async Task AddEventAsync(string entityType, Guid entityId, IEvent @event)
        {
            string serialized = JsonConvert.SerializeObject(@event, _jsonSerializerSettings);
            string filePath = GetAggregateFilePath(entityType, entityId, createFolderIfNotExist: true);
            using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var sw = new StreamWriter(fs))
            {
                await sw.WriteLineAsync(serialized);
            }
        }

        public Task<Guid[]> GetEntityIdsAsync(string entityType)
        {
            return Task.Run(() =>
            {
                string folder = GetAggregateFolder(entityType);
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

        public async Task<IEvent[]> GetEventsAsync(string entityType, Guid entityId)
        {
            string filePath = GetAggregateFilePath(entityType, entityId);
            if (!File.Exists(filePath))
            {
                return new IEvent[0];
            }

            var events = new List<IEvent>();
            using (FileStream fs = File.OpenRead(filePath))
            using (var sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    string serialized = await sr.ReadLineAsync();
                    IEvent @event = JsonConvert.DeserializeObject<IEvent>(serialized, _jsonSerializerSettings);
                    events.Add(@event);
                }
            }
            return events.ToArray();
        }

        private string GetAggregateFilePath(string entityType, Guid entityId, bool createFolderIfNotExist = false)
        {
            string folder = GetAggregateFolder(entityType, createFolderIfNotExist);
            return Path.Combine(folder, $"{entityId}.txt");
        }

        private string GetAggregateFolder(string entityType, bool createIfNotExist = false)
        {
            string folder = Path.Combine(_options.Value.Folder, entityType);
            if (createIfNotExist)
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }
    }
}
