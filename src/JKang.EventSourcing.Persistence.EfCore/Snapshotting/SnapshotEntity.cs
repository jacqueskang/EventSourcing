namespace JKang.EventSourcing.Persistence.EfCore.Snapshotting
{
    public class SnapshotEntity<TKey>
    {
        public TKey AggregateId { get; set; }
        public int AggregateVersion { get; set; }
        public string Serialized { get; set; }
    }
}
