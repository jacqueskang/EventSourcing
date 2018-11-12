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

        public Task<byte[]> GetDataAsync(string store, string container, string key)
        {
            string filePath = GetFilePath(store, container, key);
            return Task.Run(() =>
            {
                return File.ReadAllBytes(filePath);
            });
        }

        public Task<string[]> GetKeysInContainerAsync(string store, string container)
        {
            return Task.Run(() =>
            {
                string indexPath = GetIndexPath(store, container);
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

        public Task SaveAsync(string store, string container, string key, byte[] data)
        {
            GetContainerFolder(store, container, createIfNotExist: true);
            string indexPath = GetIndexPath(store, container);
            string filePath = GetFilePath(store, container, key);
            return Task.Run(() =>
            {
                File.WriteAllBytes(filePath, data);
                using (StreamWriter sw = File.AppendText(indexPath))
                {
                    sw.WriteLine(key);
                }
            });
        }

        public Task<string[]> GetContainersInStoreAsync(string store)
        {
            return Task.Run<string[]>(() =>
            {
                string storeFolder = GetStoreFolder(store);
                var di = new DirectoryInfo(storeFolder);
                if (!di.Exists)
                {
                    return new string[0];
                }

                return di.GetDirectories("*", SearchOption.TopDirectoryOnly)
                    .Select(x => x.Name)
                    .ToArray();
            });
        }

        #region private methods

        private string GetIndexPath(string store, string container)
        {
            string folder = GetContainerFolder(store, container);
            return Path.Combine(folder, "keys.txt");
        }

        private string GetFilePath(string store, string container, string key)
        {
            string folder = GetContainerFolder(store, container);
            string filePath = Path.Combine(folder, $"{key}.bin");
            return Path.ChangeExtension(filePath, _options.Value.Extension);
        }

        private string GetContainerFolder(string store, string container, bool createIfNotExist = false)
        {
            string storeFolder = GetStoreFolder(store);
            string containerFolder = Path.Combine(storeFolder, container);
            if (createIfNotExist)
            {
                Directory.CreateDirectory(containerFolder);
            }
            return containerFolder;
        }

        private string GetStoreFolder(string store)
        {
            return Path.Combine(_options.Value.Folder, store);
        }

        #endregion
    }
}
