namespace JKang.EventSourcing.Persistence.CosmosDB
{
    public class CosmosDBEventStoreOptions
    {
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }
    }
}