using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
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

        public Task<byte[]> GetDataAsync(string container, string key)
        {
            string filePath = GetFilePath(container, key);
            return Task.Run(() =>
            {
                return File.ReadAllBytes(filePath);
            });
        }

        public Task<string[]> GetKeysInContainerAsync(string container)
        {
            return Task.Run(() =>
            {
                string indexPath = GetIndexPath(container);
                if (!File.Exists(indexPath))
                {
                    return new string[0];
                }

                return File.ReadAllLines(indexPath)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();
            });
        }

        public Task SaveAsync(string container, string key, byte[] data)
        {
            GetFolder(container, createIfNotExist: true);
            string indexPath = GetIndexPath(container);
            string filePath = GetFilePath(container, key);
            return Task.Run(() =>
            {
                File.WriteAllBytes(filePath, data);
                using (StreamWriter sw = File.AppendText(indexPath))
                {
                    sw.WriteLine(key);
                }
            });
        }

        private string GetIndexPath(string container)
        {
            string folder = GetFolder(container);
            return Path.Combine(folder, "keys.txt");
        }

        private string GetFilePath(string container, string key)
        {
            string folder = GetFolder(container);
            string filePath = Path.Combine(folder, $"{key}.bin");
            return Path.ChangeExtension(filePath, _options.Value.Extension);
        }

        private string GetFolder(string container, bool createIfNotExist = false)
        {
            string folder = Path.Combine(_options.Value.Folder, container);
            if (createIfNotExist)
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }
    }
}
