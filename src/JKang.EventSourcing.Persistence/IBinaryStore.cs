using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence
{
    public interface IBinaryStore
    {
        Task SaveAsync(string container, string key, byte[] data);
        Task<string[]> GetKeysInContainerAsync(string container);
        Task<byte[]> GetDataAsync(string container, string key);
    }
}