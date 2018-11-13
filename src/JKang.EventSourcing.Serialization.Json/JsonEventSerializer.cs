using JKang.EventSourcing.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace JKang.EventSourcing.Serialization.Json
{
    class JsonEventSerializer : IEventSerializer
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            Converters = new[] { new StringEnumConverter() },
        };

        public IEvent Deserialize(string serialized)
        {
            return JsonConvert.DeserializeObject<IEvent>(serialized, _jsonSerializerSettings);
        }

        public string Serialize(IEvent @event)
        {
            return JsonConvert.SerializeObject(@event, _jsonSerializerSettings);
        }
    }
}
