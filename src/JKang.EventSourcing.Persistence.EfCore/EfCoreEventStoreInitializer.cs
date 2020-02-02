using JKang.EventSourcing.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class EfCoreEventStoreInitializer<TEventDbContext, TAggregate, TKey>
        : IEventStoreInitializer<TAggregate, TKey>
        where TEventDbContext : DbContext, IEventDbContext<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly TEventDbContext _context;

        public EfCoreEventStoreInitializer(TEventDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            return _context.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}
