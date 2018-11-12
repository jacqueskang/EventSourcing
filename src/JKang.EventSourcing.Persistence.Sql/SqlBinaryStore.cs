using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.Sql
{
    public class SqlBinaryStore<TDbContext> : IBinaryStore
    {
        public SqlBinaryStore()
        {

        }

        public Task<string[]> GetContainersInStoreAsync(string store)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetDataAsync(string store, string container, string key)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetKeysInContainerAsync(string store, string container)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(string store, string container, string key, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
