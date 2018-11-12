namespace JKang.EventSourcing.Persistence.FileSystem
{
    public class FileSystemBinaryStoreOptions
    {
        public string Folder { get; set; }
        public string Extension { get; set; } = ".bin";
    }
}
