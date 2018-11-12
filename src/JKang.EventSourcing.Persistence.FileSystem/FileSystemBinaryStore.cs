using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.FileSystem
{
    public class FileSystemBinaryStore : IBinaryStore
    {
        private readonly IOptions<FileSystemBinaryStoreOptions> _options;

        public FileSystemBinaryStore(IOptions<FileSystemBinaryStoreOptions> options)
        {
            _options = options;
        }

        public Task SaveAsync(string container, string key, byte[] data)
        {
            string folder = Path.Combine(_options.Value.Folder, container);
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, $"{key}.bin");
            return Task.Run(() => File.WriteAllBytes(filePath, data));
        }
    }
}
