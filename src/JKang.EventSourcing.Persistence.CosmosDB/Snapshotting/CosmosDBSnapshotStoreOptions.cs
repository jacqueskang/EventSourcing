namespace JKang.EventSourcing.Persistence.CosmosDB.Snapshotting
{
    public class CosmosDBSnapshotStoreOptions
    {
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }
    }
}