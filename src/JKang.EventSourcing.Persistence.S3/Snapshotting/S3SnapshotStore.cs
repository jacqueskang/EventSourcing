using Amazon.S3;
using Amazon.S3.Model;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Options;
using JKang.EventSourcing.Snapshotting.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.EventSourcing.Persistence.S3.Snapshotting
{
    public class S3SnapshotStore<TAggregate, TKey> : ISnapshotStore<TAggregate, TKey>
        where TAggregate : IAggregate<TKey>
    {
        private readonly IAggregateOptionsMonitor<TAggregate, TKey, S3SnapshotStoreOptions> _monitor;
        private readonly IAmazonS3 _client;

        public S3SnapshotStore(
            IAggregateOptionsMonitor<TAggregate, TKey, S3SnapshotStoreOptions> monitor,
            IAmazonS3 client)
        {
            if (monitor is null)
            {
                throw new System.ArgumentNullException(nameof(monitor));
            }

            _monitor = monitor;
            _client = client;
        }

        public async Task AddSnapshotAsync(IAggregateSnapshot<TKey> snapshot, CancellationToken cancellationToken = default)
        {
            if (snapshot is null)
            {
                throw new System.ArgumentNullException(nameof(snapshot));
            }

            string json = JsonConvert.SerializeObject(snapshot, Defaults.JsonSerializerSettings);

            S3SnapshotStoreOptions options = _monitor.AggregateOptions;
            var request = new PutObjectRequest
            {
                BucketName = options.BucketName,
                Key = $"{options.Prefix}/{snapshot.AggregateId}/{snapshot.AggregateVersion}.json",
                ContentBody = json
            };
            await _client.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IAggregateSnapshot<TKey>> FindLastSnapshotAsync(TKey aggregateId, int maxVersion, CancellationToken cancellationToken = default)
        {
            S3SnapshotStoreOptions options = _monitor.AggregateOptions;
            string prefix = $"{options.Prefix}/{aggregateId}/";
            ListObjectsV2Response response = await _client.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = options.BucketName,
                Prefix = prefix
            });

            var objects = response.S3Objects
                .Select(x => x.Key)
                .Select(x => new { Key = x, FileName = x.Substring(prefix.Length).Split('.').First() })
                .Where(x => int.TryParse(x.FileName, out int _))
                .Select(x => new { x.Key, Version = int.Parse(x.FileName) })
                .Where(x => x.Version <= maxVersion)
                .OrderByDescending(x => x.Version)
                .ToList();
            if (objects.Count == 0)
            {
                return null;
            }

            string json;
            var request2 = new GetObjectRequest
            {
                BucketName = options.BucketName,
                Key = objects.First().Key
            };
            using (GetObjectResponse response2 = await _client.GetObjectAsync(request2))
            using (Stream responseStream = response2.ResponseStream)
            using (var reader = new StreamReader(responseStream))
            {
                json = reader.ReadToEnd(); // Now you process the response body.
            }
            IAggregateSnapshot<TKey> snapshot = JsonConvert.DeserializeObject<IAggregateSnapshot<TKey>>(json, Defaults.JsonSerializerSettings);
            return snapshot;
        }
    }
}
