using JKang.EventSourcing.Events;

namespace JKang.EventSourcing.Serialization
{
    public interface ITextEventSerializer
    {
        string Serialize(IEvent @event);
        IEvent Deserialize(string serialized);
    }
}
