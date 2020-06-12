using JKang.EventSourcing.Domain;
using Microsoft.EntityFrameworkCore;

namespace JKang.EventSourcing.Persistence.EfCore.Snapshotting
{
    public interface ISnapshotDbContext<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        DbSet<SnapshotEntity<TKey>> GetSnapshotDbSet();
    }
}
