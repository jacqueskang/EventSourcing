using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Snapshotting.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.EfCore.Snapshotting
{
    public class EfCoreSnapshotStoreInitializer<TSnapshotDbContext, TAggregate, TKey>
        : ISnapshotStoreInitializer<TAggregate, TKey>
        where TSnapshotDbContext : DbContext, ISnapshotDbContext<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly TSnapshotDbContext _context;

        public EfCoreSnapshotStoreInitializer(TSnapshotDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            return _context.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}
