using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class DatabaseEventStoreInitializer<TEventSourcingDbContext, TAggregate, TKey>
        : IEventStoreInitializer<TAggregate, TKey>
        where TEventSourcingDbContext : DbContext, IEventSourcingDbContext<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly TEventSourcingDbContext _context;

        public DatabaseEventStoreInitializer(TEventSourcingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            return _context.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}
