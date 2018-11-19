using AutoFixture.Xunit2;
using JKang.EventSourcing.Events;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace JKang.EventSourcing.Serialization.Json.Tests
{
    public class JsonObjectSerializerTest
    {
        private readonly JsonObjectSerializer _sut = new JsonObjectSerializer();

        [Theory, AutoData]
        public void SerializeAndDeserialize(TestEvent expected)
        {
            string serialized = _sut.Serialize(expected);
            IAggregateEvent actual = _sut.Deserialize<IAggregateEvent>(serialized);
            Assert.NotNull(actual);
            Assert.IsType<TestEvent>(actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.DateTime, actual.DateTime);
            Assert.Equal(expected.AggregateId, actual.AggregateId);
            Assert.Equal(expected.AggregateVersion, actual.AggregateVersion);
            Assert.Equal(expected.CustomProperty1, (actual as TestEvent).CustomProperty1);
            Assert.Equal(expected.CustomProperty2, (actual as TestEvent).CustomProperty2);
        }
    }
}
