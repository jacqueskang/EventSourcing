using JKang.EventSourcing.Events;
using System;

namespace JKang.EventSourcing.Serialization.Json.Tests
{
    public class TestEvent : AggregateEvent
    {
        public TestEvent(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion, string customProperty)
            : base(id, dateTime, aggregateId, aggregateVersion)
        {
            CustomProperty = customProperty;
        }

        public string CustomProperty { get; }
    }
}
