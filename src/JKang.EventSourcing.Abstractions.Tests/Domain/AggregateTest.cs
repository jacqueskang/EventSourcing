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
            Assert.Null(changeset.Snapshot);
        }

        [Theory, AutoData]
        public void Constructor_CreateFromSnapshotWithSavedEvents(Guid aggregateId)
        {
            // arrange;
            var snapshot = new GiftCardSnapshot(aggregateId, 10, 70);
            var history = new IAggregateEvent<Guid>[]
            {
                new GiftCardDebited(aggregateId, 11, DateTime.UtcNow.AddDays(-5), 30)
            };

            // act
            var sut = new GiftCard(aggregateId, snapshot, history);
            IAggregateChangeset<Guid> changeset = sut.GetChangeset();

            // assert
            Assert.Equal(70 - 30, sut.Balance);
            Assert.Equal(aggregateId, sut.Id);
            Assert.Equal(11, sut.Version);
            Assert.Single(sut.Events);
            Assert.Empty(changeset.Events);
            Assert.Null(changeset.Snapshot);
        }

        [Theory, AutoData]
        public void TakeSnapshot(Guid aggregateId)
        {
            // arrange;
            var savedEvents = new IAggregateEvent<Guid>[]
            {
                new GiftCardCreated(aggregateId, DateTime.UtcNow.AddDays(-10), 100),
                new GiftCardDebited(aggregateId, 2, DateTime.UtcNow.AddDays(-5), 30),
                new GiftCardDebited(aggregateId, 3, DateTime.UtcNow.AddDays(-2), 20)
            };
            var sut = new GiftCard(aggregateId, savedEvents);

            // act
            sut.TakeSnapshot();

            // assert
            Assert.Equal(3, sut.Version);
            IAggregateChangeset<Guid> changeset = sut.GetChangeset();
            Assert.NotNull(changeset.Snapshot);
            Assert.IsType<GiftCardSnapshot>(changeset.Snapshot);
            var snapshot = changeset.Snapshot as GiftCardSnapshot;
            Assert.Equal(aggregateId, snapshot.AggregateId);
            Assert.Equal(3, snapshot.AggregateVersion);
            Assert.Equal(sut.Balance, snapshot.Balance);
        }
    }
}
