namespace JKang.EventSourcing.Persistence.FileSystem
{
    public class TextFileEventStoreOptions
    {
        public string Folder { get; set; }
        public string EventSeparator { get; set; } = "\r\n";
    }
}
