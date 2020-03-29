namespace Microsoft.Extensions.DependencyInjection
{
    public class S3SnapshotStoreOptions
    {
        public string BucketName { get; set; }
        public string Prefix { get; set; }
    }
}