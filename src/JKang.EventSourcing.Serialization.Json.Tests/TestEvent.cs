using JKang.EventSourcing.Events;
using System;

namespace JKang.EventSourcing.Serialization.Json.Tests
{
    public class TestEvent : AggregateEvent<Guid>
    {
        public TestEvent(Guid aggregateId, int aggregateVersion, string customProperty)
            : base(aggregateId, aggregateVersion)
        {
            CustomProperty = customProperty;
        }

        public string CustomProperty { get; }
    }
}
