using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Options;
using JKang.EventSourcing.Snapshotting.Persistence;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.FileSystem.Snapshotting
{
    public class TextFileSnapshotStore<TAggregate, TKey> : ISnapshotStore<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly TextFileSnapshotStoreOptions _options;

        public TextFileSnapshotStore(
            IAggregateOptionsMonitor<TAggregate, TKey, TextFileSnapshotStoreOptions> monitor)
        {
            if (monitor is null)
            {
                throw new ArgumentNullException(nameof(monitor));
            }

            _options = monitor.AggregateOptions;
        }

        public Task AddSnapshotAsync(IAggregateSnapshot<TKey> snapshot,
            CancellationToken cancellationToken = default)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            string serialized = JsonConvert.SerializeObject(snapshot, Defaults.JsonSerializerSettings);
            string filePath = GetFilePath(snapshot.AggregateId, snapshot.AggregateVersion);
            File.WriteAllText(filePath, serialized);
            return Task.CompletedTask;
        }

        public Task<IAggregateSnapshot<TKey>> FindLastSnapshotAsync(TKey aggregateId, int maxVersion,
            CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(_options.Folder))
            {
                return Task.FromResult(null as IAggregateSnapshot<TKey>);
            }

            int latestVersion = Directory
                .GetFiles(_options.Folder, "*.snapshot")
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => x.Split('.').LastOrDefault())
                .Select(x => int.TryParse(x, NumberStyles.Integer, CultureInfo.InvariantCulture, out int version) ? version : -1)
                .Where(x => x <= maxVersion)
                .OrderByDescending(x => x)
                .FirstOrDefault();

            if (latestVersion < 1)
            {
                return Task.FromResult(null as IAggregateSnapshot<TKey>);
            }

            string filePath = GetFilePath(aggregateId, latestVersion);
            string serialized = File.ReadAllText(filePath);
            IAggregateSnapshot<TKey> snapshot = JsonConvert.DeserializeObject<IAggregateSnapshot<TKey>>(
                serialized, Defaults.JsonSerializerSettings);
            return Task.FromResult(snapshot);
        }

        private string GetFilePath(TKey aggregateId, int version)
            => Path.Combine(_options.Folder, $"{aggregateId}.{version.ToString(CultureInfo.InvariantCulture)}.snapshot");
    }
}
