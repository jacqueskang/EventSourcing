using AutoFixture.Xunit2;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.TestingFixtures;
using System;
using System.Linq;
using Xunit;

namespace JKang.EventSourcing.Abstractions.Tests
{
    public class AggregateTest
    {
        [Theory, AutoData]
        public void Constructor_CreateNew(decimal initialCredit)
        {
            // act
            var sut = new GiftCard(initialCredit);
            IAggregateChangeset<Guid> changeset = sut.GetChangeset();

            // assert
            Assert.Equal(1, sut.Version);
            Assert.Single(sut.Events);
            var createdEvent = sut.Events.SingleOrDefault() as GiftCardCreated;
            Assert.NotNull(createdEvent);
            Assert.Equal(1, createdEvent.AggregateVersion);
            Assert.Single(changeset.Events);
            Assert.Same(createdEvent, changeset.Events.Single());
        }

        [Theory, AutoData]
        public void Constructor_CreateFromSavedEvents(Guid aggregateId)
        {
            // arrange;
            var history = new IAggregateEvent<Guid>[]
            {
                new GiftCardCreated(aggregateId, DateTime.UtcNow.AddDays(-10), 100),
                new GiftCardDebited(aggregateId, 2, DateTime.UtcNow.AddDays(-5), 30)
            };

            // act
            var sut = new GiftCard(aggregateId, history);
            IAggregateChangeset<Guid> changeset = sut.GetChangeset();

            // assert
            Assert.Equal(100 - 30, sut.Balance);
            Assert.Equal(aggregateId, sut.Id);
            Assert.Equal(2, sut.Version);
            Assert.Equal(2, sut.Events.Count());
            Assert.Empty(changeset.Events);
        }

        [Theory, AutoData]
        public void Constructor_CreateFromSnapshotWithSavedEvents(Guid aggregateId)
        {
            // arrange;
            var history = new IAggregateEvent<Guid>[]
            {
                new GiftCardSnapshot(aggregateId, 10, DateTime.UtcNow.AddDays(-10), 70),
                new GiftCardDebited(aggregateId, 11, DateTime.UtcNow.AddDays(-5), 30)
            };

            // act
            var sut = new GiftCard(aggregateId, history);
            IAggregateChangeset<Guid> changeset = sut.GetChangeset();

            // assert
            Assert.Equal(70 - 30, sut.Balance);
            Assert.Equal(aggregateId, sut.Id);
            Assert.Equal(11, sut.Version);
            Assert.Equal(2, sut.Events.Count());
            Assert.Empty(changeset.Events);
        }
    }
}
