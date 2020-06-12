using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace JKang.EventSourcing.Persistence.EfCore
{
    internal static class Defaults
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            Converters = new[] { new StringEnumConverter() },
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
        };
    }
}
