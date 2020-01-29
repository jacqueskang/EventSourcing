using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.FileSystem
{
    public class TextFileEventStoreInitializer<TAggregate, TKey>
        : IEventStoreInitializer<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly TextFileEventStoreOptions _options;

        public TextFileEventStoreInitializer(
            IAggregateOptionsMonitor<TAggregate, TKey, TextFileEventStoreOptions> monitor)
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
