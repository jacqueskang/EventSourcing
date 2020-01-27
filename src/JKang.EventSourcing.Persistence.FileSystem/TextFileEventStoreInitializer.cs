using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.FileSystem
{
    public class TextFileEventStoreInitializer<TAggregate, TAggregateKey>
        : IEventStoreInitializer<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
    {
        private readonly TextFileEventStoreOptions _options;

        public TextFileEventStoreInitializer(
            IAggregateOptionsMonitor<TAggregate, TAggregateKey, TextFileEventStoreOptions> monitor)
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
