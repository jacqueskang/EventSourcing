using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IBinaryStore
    {
        Task SaveAsync(string store, string container, string key, byte[] data);
        Task<string[]> GetKeysInContainerAsync(string store, string container);
        Task<byte[]> GetDataAsync(string store, string container, string key);
        Task<string[]> GetContainersInStoreAsync(string store);
    }
}