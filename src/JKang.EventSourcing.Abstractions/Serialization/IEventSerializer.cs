using JKang.EventSourcing.Events;

namespace JKang.EventSourcing.Serialization
{
    public interface IEventSerializer
    {
        string Serialize(IEvent @event);
        IEvent Deserialize(string serialized);
    }
}
