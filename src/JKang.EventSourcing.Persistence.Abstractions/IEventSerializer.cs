using JKang.EventSourcing.Events;

namespace JKang.EventSourcing.Persistence
{
    public interface IEventSerializer
    {
        byte[] Serialize(IEvent @event);
        IEvent Deserialize(byte[] serialized);
    }
}
