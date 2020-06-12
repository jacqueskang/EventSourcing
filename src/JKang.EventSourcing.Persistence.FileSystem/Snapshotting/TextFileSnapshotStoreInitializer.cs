using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Options;
using JKang.EventSourcing.Snapshotting.Persistence;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.FileSystem.Snapshotting
{
    public class TextFileSnapshotStoreInitializer<TAggregate, TKey>
        : ISnapshotStoreInitializer<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly TextFileSnapshotStoreOptions _options;

        public TextFileSnapshotStoreInitializer(
            IAggregateOptionsMonitor<TAggregate, TKey, TextFileSnapshotStoreOptions> monitor)
        {
            if (monitor is null)
            {
                throw new ArgumentNullException(nameof(monitor));
            }

            _options = monitor.AggregateOptions;
        }

        public Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(_options.Folder);
            return Task.CompletedTask;
        }
    }
}
