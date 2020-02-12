using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class EfCoreEventStore<TEventDbContext, TAggregate, TKey> : IEventStore<TAggregate, TKey>
        where TEventDbContext : DbContext, IEventDbContext<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly TEventDbContext _context;

        public EfCoreEventStore(TEventDbContext context)
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

            string serialized = JsonConvert.SerializeObject(@event, Defaults.JsonSerializerSettings);
            var entity = new EventEntity<TKey>
            {
                AggregateId = @event.AggregateId,
                AggregateVersion = @event.AggregateVersion,
                Timestamp = @event.Timestamp,
                Serialized = serialized
            };
            await _context.GetEventDbSet().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<TKey[]> GetAggregateIdsAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.GetEventDbSet()
                .Select(x => x.AggregateId)
                .Distinct()
                .ToArrayAsync(cancellationToken);
        }

        public async Task<IAggregateEvent<TKey>[]> GetEventsAsync(
            TKey aggregateId,
            int minVersion, int maxVersion,
            CancellationToken cancellationToken = default)
        {
            List<string> serializedEvents = await _context.GetEventDbSet()
                .Where(x => x.AggregateId.Equals(aggregateId))
                .Where(x => x.AggregateVersion >= minVersion)
                .Where(x => x.AggregateVersion <= maxVersion)
                .OrderBy(x => x.AggregateVersion)
                .Select(x => x.Serialized)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return serializedEvents
                .Select(x => JsonConvert.DeserializeObject<IAggregateEvent<TKey>>(x, Defaults.JsonSerializerSettings))
                .ToArray();
        }
    }
}
