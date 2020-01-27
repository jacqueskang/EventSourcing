using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class DatabaseEventStoreInitializer<TEventSourcingDbContext, TAggregate, TAggregateKey>
        : IEventStoreInitializer<TAggregate, TAggregateKey>
        where TEventSourcingDbContext : DbContext, IEventSourcingDbContext<TAggregate, TAggregateKey>
        where TAggregate : IAggregate<TAggregateKey>
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
