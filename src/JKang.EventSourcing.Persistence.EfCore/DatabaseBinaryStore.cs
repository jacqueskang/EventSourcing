using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class DatabaseBinaryStore<TDbContext> : IBinaryStore
        where TDbContext : DbContext, IEventSourcingDbContext
    {
        private readonly TDbContext _context;

        public DatabaseBinaryStore(TDbContext context)
        {
            _context = context;
        }

        public Task<string[]> GetContainersInStoreAsync(string store)
        {
            return _context.Events
                .Where(x => x.Store == store)
                .Select(x => x.Container)
                .Distinct()
                .ToArrayAsync();
        }

        public Task<byte[]> GetDataAsync(string store, string container, string key)
        {
            return _context.Events
                .Where(x => x.Store == store)
                .Where(x => x.Container == container)
                .Where(x => x.Key == key)
                .Select(x => x.Data)
                .FirstOrDefaultAsync();
        }

        public Task<string[]> GetKeysInContainerAsync(string store, string container)
        {
            return _context.Events
                .Where(x => x.Store == store)
                .Where(x => x.Container == container)
                .Select(x => x.Key)
                .ToArrayAsync();
        }

        public async Task SaveAsync(string store, string container, string key, byte[] data)
        {
            EfEvent record = await _context.Events
                .Where(x => x.Store == store)
                .Where(x => x.Container == container)
                .Where(x => x.Key == key)
                .FirstOrDefaultAsync();

            if (record == null)
            {
                record = new EfEvent
                {
                    Store = store,
                    Container = container,
                    Key = key,
                    Data = data,
                };
                await _context.Events.AddAsync(record);
            }
            else
            {
                record.Data = data;
            }

            await _context.SaveChangesAsync();
        }
    }
}
