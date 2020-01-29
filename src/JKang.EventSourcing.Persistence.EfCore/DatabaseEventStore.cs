using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class DatabaseEventStore<TDbContext, TAggregate, TKey> : IEventStore<TAggregate, TKey>
        where TDbContext : DbContext, IEventSourcingDbContext<TAggregate, TKey>
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
        private readonly TDbContext _context;

        public DatabaseEventStore(TDbContext context)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
        }

        public async Task AddEventAsync(IAggregateEvent<TKey> @event,
            CancellationToken cancellationToken = default)
        {
            if (@event is null)
            {
                throw new System.ArgumentNullException(nameof(@event));
            }

            string serialized = JsonConvert.SerializeObject(@event, _jsonSerializerSettings);
            var entity = new EventEntity<TKey>
            {
                AggregateId = @event.AggregateId,
                AggregateVersion = @event.AggregateVersion,
                Timestamp = @event.Timestamp,
                Serialized = serialized
            };
            await _context.GetDbSet().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<TKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.GetDbSet()
                .Select(x => x.AggregateId)
                .Distinct()
                .ToArrayAsync(cancellationToken);
        }

        public async Task<IAggregateEvent<TKey>[]> GetEventsAsync(
            TKey aggregateId,
            int skip = 0,
            CancellationToken cancellationToken = default)
        {
            List<string> serializedEvents = await _context.GetDbSet()
                .Where(x => x.AggregateId.Equals(aggregateId))
                .OrderBy(x => x.AggregateVersion)
                .Skip(skip)
                .Select(x => x.Serialized)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return serializedEvents
                .Select(x => JsonConvert.DeserializeObject<IAggregateEvent<TKey>>(x, _jsonSerializerSettings))
                .ToArray();
        }
    }
}
