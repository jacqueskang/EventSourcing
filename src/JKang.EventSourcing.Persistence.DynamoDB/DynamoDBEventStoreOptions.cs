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
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new InvalidOperationException("UseLocalDB is false");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            }

            return new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = ServiceURL.ToString()
            });
        }
    }
}