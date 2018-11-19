using AutoFixture.Xunit2;
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
            TestEvent actual = _sut.Deserialize<TestEvent>(serialized);
            Assert.NotNull(actual);
            Assert.Equal(expected.AggregateId, actual.AggregateId);
            Assert.Equal(expected.AggregateVersion, actual.AggregateVersion);
            Assert.Equal(expected.CustomProperty, actual.CustomProperty);
        }
    }
}
