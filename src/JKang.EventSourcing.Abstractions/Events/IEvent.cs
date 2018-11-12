using System;

namespace JKang.EventSourcing.Events
{
    public interface IEvent
    {
        Guid Id { get; }

        DateTime DateTime { get; }
    }
}
