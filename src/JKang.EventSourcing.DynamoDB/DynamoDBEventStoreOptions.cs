using Amazon.DynamoDBv2;
using System;

namespace JKang.EventSourcing.Persistence.DynamoDB
{
    public class DynamoDBEventStoreOptions
    {
        public string TableName { get; set; }

        public bool UseLocalDB { get; set; }

        /// <summary>
        /// Required if UseLocalDB is True
        /// </summary>
        public Uri ServiceURL { get; set; }

        internal AmazonDynamoDBClient CreateLocalDBClient()
        {
            if (!UseLocalDB)
            {
                throw new InvalidOperationException("UseLocalDB is false");
            }
            return new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = ServiceURL.AbsoluteUri
            });
        }
    }
}