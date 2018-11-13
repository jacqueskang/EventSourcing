using JKang.EventSourcing.Events;

namespace JKang.EventSourcing.Serialization
{
    public interface IObjectSerializer
    {
        string Serialize(object obj);
        T Deserialize<T>(string serialized);
    }
}
