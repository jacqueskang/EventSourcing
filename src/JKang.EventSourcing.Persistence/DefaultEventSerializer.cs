using JKang.EventSourcing.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace JKang.EventSourcing.Persistence
{
    public class DefaultEventSerializer : IEventSerializer
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            Converters = new[] { new StringEnumConverter() },
        };

        public byte[] Serialize(IEvent @event)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, _settings));
        }
    }
}
