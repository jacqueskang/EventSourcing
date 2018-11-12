namespace JKang.EventSourcing.Persistence.EfCore
{
    public class EfEvent
    {
        public string Store { get; set; }
        public string Container { get; set; }
        public string Key { get; set; }
        public byte[] Data { get; set; }
    }
}
