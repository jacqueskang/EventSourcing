using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Snapshotting.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.EfCore.Snapshotting
{
    public class EfCoreSnapshotStore<TSnapshotDbContext, TAggregate, TKey> : ISnapshotStore<TAggregate, TKey>
        where TSnapshotDbContext : DbContext, ISnapshotDbContext<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly TSnapshotDbContext _context;

        public EfCoreSnapshotStore(TSnapshotDbContext context)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
        }

        public async Task AddSnapshotAsync(IAggregateSnapshot<TKey> snapshot,
            CancellationToken cancellationToken = default)
        {
            if (snapshot is null)
            {
                throw new System.ArgumentNullException(nameof(snapshot));
            }

            string serialized = JsonConvert.SerializeObject(snapshot, Defaults.JsonSerializerSettings);
            var entity = new SnapshotEntity<TKey>
            {
                AggregateId = snapshot.AggregateId,
                AggregateVersion = snapshot.AggregateVersion,
                Serialized = serialized
            };
            await _context.GetSnapshotDbSet().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IAggregateSnapshot<TKey>> FindLastSnapshotAsync(TKey aggregateId, int maxVersion,
            CancellationToken cancellationToken = default)
        {
            string serialized = await _context.GetSnapshotDbSet()
                .Where(x => x.AggregateId.Equals(aggregateId))
                .Where(x => x.AggregateVersion <= maxVersion)
                .OrderByDescending(x => x.AggregateVersion)
                .Select(x => x.Serialized)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return serialized == null
                ? null
                : JsonConvert.DeserializeObject<IAggregateSnapshot<TKey>>(serialized, Defaults.JsonSerializerSettings);
        }
    }
}
