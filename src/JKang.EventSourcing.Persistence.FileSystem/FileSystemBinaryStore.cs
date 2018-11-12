using System;
using System.IO;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.FileSystem
{
    public class FileSystemBinaryStore : IBinaryStore
    {
        public Task SaveAsync(string container, string key, byte[] data)
        {
            Directory.CreateDirectory(container);
            string filePath = Path.Combine(container, $"{key}.bin");
            return Task.Run(() => File.WriteAllBytes(filePath, data));
        }
    }
}
