using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Serialization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class DatabaseEventStore<TDbContext, TAggregate> : IEventStore<TAggregate>
        where TDbContext : DbContext, IEventSourcingDbContext<TAggregate>
        where TAggregate : Aggregate
    {
        private readonly TDbContext _context;
        private readonly IEventSerializer _eventSerializer;

        public DatabaseEventStore(
            TDbContext context,
            IEventSerializer eventSerializer)
        {
            _context = context;
            _eventSerializer = eventSerializer;
        }

        public async Task AddEventAsync(AggregateEvent @event)
        {
            string serialized = _eventSerializer.Serialize(@event);
            var entity = new EventEntity
            {
                Id = @event.Id,
                AggregateId = @event.AggregateId,
                AggregateVersion = @event.AggregateVersion,
                Serialized = serialized
            };
            await _context.GetDbSet().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task<Guid[]> GetAggregateIdsAsync()
        {
            return _context.GetDbSet()
                .Select(x => x.AggregateId)
                .Distinct()
                .ToArrayAsync();
        }

        public async Task<AggregateEvent[]> GetEventsAsync(Guid aggregateId)
        {
            List<string> serializedEvents = await _context.GetDbSet()
                .Where(x => x.AggregateId == aggregateId)
                .OrderBy(x => x.AggregateVersion)
                .Select(x => x.Serialized)
                .ToListAsync();

            return serializedEvents
                .Select(x => _eventSerializer.Deserialize(x) as AggregateEvent)
                .ToArray();
        }
    }
}
