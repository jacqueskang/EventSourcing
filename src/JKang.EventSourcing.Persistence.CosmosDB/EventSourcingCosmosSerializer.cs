using JKang.EventSourcing.Serialization.Json;
using Microsoft.Azure.Cosmos;
using System.IO;
using System.Text;

namespace JKang.EventSourcing.Persistence.CosmosDB
{
    public class EventSourcingCosmosSerializer : CosmosSerializer
    {
        private readonly JsonObjectSerializer _serializer = new JsonObjectSerializer();

        public override T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)stream;
                }

                using (var sr = new StreamReader(stream))
                {
                    return _serializer.Deserialize<T>(sr.ReadToEnd());
                }
            }
        }

        public override Stream ToStream<T>(T input)
        {
            string serialized = _serializer.Serialize(input);
            return new MemoryStream(Encoding.UTF8.GetBytes(serialized));
        }
    }
}
