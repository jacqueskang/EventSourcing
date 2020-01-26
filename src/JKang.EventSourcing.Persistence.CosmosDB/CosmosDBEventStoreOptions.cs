using System;

namespace JKang.EventSourcing.Persistence.DynamoDB
{
    public class CosmosDBEventStoreOptions
    {
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }
    }
}