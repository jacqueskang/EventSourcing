using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JKang.EventSourcing.Serialization.Json.Tests
{
    public class TestEvent : AggregateEvent
    {
        public TestEvent(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion,
            string customProperty1, IEnumerable<string> customProperty2)
            : base(id, dateTime, aggregateId, aggregateVersion)
        {
            CustomProperty1 = customProperty1;
            CustomProperty2 = customProperty2.ToList().AsReadOnly();
        }

        public string CustomProperty1 { get; }
        public IEnumerable<string> CustomProperty2 { get; }
    }
}
