using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Snapshotting.Persistence;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public abstract class CachedAggregateRepository<TAggregate, TKey>
        : AggregateRepository<TAggregate, TKey>
        where TAggregate : class, IAggregate<TKey>
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _cacheOptions;
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            Converters = new[] { new StringEnumConverter() },
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
        };

        protected CachedAggregateRepository(
            IEventStore<TAggregate, TKey> eventStore,
            ISnapshotStore<TAggregate, TKey> snapshotStore,
            IDistributedCache cache,
            DistributedCacheEntryOptions cacheOptions)
            : base(eventStore, snapshotStore)
        {
            _cache = cache;
            _cacheOptions = cacheOptions;
        }

        protected override async Task<TAggregate> FindAggregateAsync(
            TKey id,
            bool ignoreSnapshot = false,
            int version = -1,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string key = GetCacheKey(id, version);
                string serialized = await _cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(serialized))
                {
                    return DeserializeAggregate(serialized);
                }
            }
            catch { }

            TAggregate aggregate = await base.FindAggregateAsync(id, ignoreSnapshot, version, cancellationToken)
                .ConfigureAwait(false);

            if (aggregate != null)
            {
                await CacheAsync(aggregate, version, cancellationToken).ConfigureAwait(false);
            }

            return aggregate;
        }

        protected override async Task<IAggregateChangeset<TKey>> SaveAggregateAsync(
            TAggregate aggregate,
            CancellationToken cancellationToken = default)
        {
            if (aggregate is null)
            {
                throw new System.ArgumentNullException(nameof(aggregate));
            }

            IAggregateChangeset<TKey> changeset = await base.SaveAggregateAsync(aggregate, cancellationToken)
                .ConfigureAwait(false);

            await CacheAsync(aggregate, -1, cancellationToken).ConfigureAwait(false);

            return changeset;
        }

        private async Task CacheAsync(TAggregate aggregate, int version,
            CancellationToken cancellationToken)
        {
            string serialized = SerializeAggregate(aggregate);
            string key = GetCacheKey(aggregate.Id, version);
            await _cache.SetStringAsync(key, serialized, _cacheOptions, cancellationToken).ConfigureAwait(false);
        }

        protected virtual string GetCacheKey(TKey id, int version) => $"{typeof(TAggregate).FullName}_{id}_{version}";

        protected virtual TAggregate DeserializeAggregate(string serialized)
        {
            CachedAggregate cached = JsonConvert.DeserializeObject<CachedAggregate>(serialized, _jsonSerializerSettings);

            if (cached.Snapshot == null)
            {
                return Activator.CreateInstance(typeof(TAggregate), cached.Id, cached.Events) as TAggregate;
            }
            else
            {
                return Activator.CreateInstance(typeof(TAggregate), cached.Id, cached.Snapshot, cached.Events) as TAggregate;
            }
        }

        protected virtual string SerializeAggregate(TAggregate aggregate)
        {
            var cached = new CachedAggregate
            {
                Id = aggregate.Id,
                Snapshot = aggregate.Snapshot,
                Events = aggregate.Events
            };
            return JsonConvert.SerializeObject(cached, _jsonSerializerSettings);
        }

        internal class CachedAggregate
        {
            public TKey Id { get; set; }
            public IAggregateSnapshot<TKey> Snapshot { get; set; }
            public IEnumerable<IAggregateEvent<TKey>> Events { get; set; }
        }
    }
}
