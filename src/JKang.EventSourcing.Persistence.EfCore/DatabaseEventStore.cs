using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class DatabaseEventStore<TDbContext, TAggregate, TAggregateKey> : IEventStore<TAggregate, TAggregateKey>
        where TDbContext : DbContext, IEventSourcingDbContext<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
    {
        private readonly TDbContext _context;
        private readonly IObjectSerializer _eventSerializer;

        public DatabaseEventStore(
            TDbContext context,
            IObjectSerializer eventSerializer)
        {
            _context = context;
            _eventSerializer = eventSerializer;
        }

        public async Task AddEventAsync(IAggregateEvent<TAggregateKey> @event)
        {
            string serialized = _eventSerializer.Serialize(@event);
            var entity = new EventEntity<TAggregateKey>
            {
                AggregateId = @event.AggregateId,
                AggregateVersion = @event.AggregateVersion,
                Timestamp = @event.Timestamp,
                Serialized = serialized
            };
            await _context.GetDbSet().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task<TAggregateKey[]> GetAggregateIdsAsync()
        {
            return _context.GetDbSet()
                .Select(x => x.AggregateId)
                .Distinct()
                .ToArrayAsync();
        }

        public async Task<IAggregateEvent<TAggregateKey>[]> GetEventsAsync(TAggregateKey aggregateId)
        {
            List<string> serializedEvents = await _context.GetDbSet()
                .Where(x => x.AggregateId.Equals(aggregateId))
                .OrderBy(x => x.AggregateVersion)
                .Select(x => x.Serialized)
                .ToListAsync();

            return serializedEvents
                .Select(x => _eventSerializer.Deserialize<IAggregateEvent<TAggregateKey>>(x))
                .ToArray();
        }
    }
}
