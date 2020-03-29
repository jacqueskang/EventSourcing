namespace JKang.EventSourcing.TestingWebApp
{
    public enum PersistenceMode
    {
        FileSystem,
        EfCore,
        DynamoDB,
        CosmosDB,
        S3
    }
}
