using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Text;

namespace JKang.EventSourcing.Persistence.CosmosDB
{
    public class EventSourcingCosmosSerializer : CosmosSerializer
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            Converters = new[] { new StringEnumConverter() },
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
        };

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
                    string serialized = sr.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(serialized, _jsonSerializerSettings);
                }
            }
        }

        public override Stream ToStream<T>(T input)
        {
            string serialized = JsonConvert.SerializeObject(input, _jsonSerializerSettings);
            return new MemoryStream(Encoding.UTF8.GetBytes(serialized));
        }
    }
}
