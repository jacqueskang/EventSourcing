using AutoFixture.Xunit2;
using JKang.EventSourcing.Events;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Snapshotting.Persistence;
using JKang.EventSourcing.TestingFixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JKang.EventSourcing.Abstractions.Tests.Persistence
{
    public class AggregateRepositoryTest
    {
        private readonly Mock<IEventStore<GiftCard, Guid>> _eventStore;
        private readonly Mock<ISnapshotStore<GiftCard, Guid>> _snapshotStore;
        private readonly GiftCardRepository _sut;

        public AggregateRepositoryTest()
        {
            _eventStore = new Mock<IEventStore<GiftCard, Guid>>();
            _snapshotStore = new Mock<ISnapshotStore<GiftCard, Guid>>();
            _sut = new GiftCardRepository(_eventStore.Object, _snapshotStore.Object);
        }

        [Theory, AutoData]
        public async Task FindAggregateAsync_NoEvent_ReturnNull(Guid id)
        {
            // arrange
            _eventStore
                .Setup(x => x.GetEventsAsync(id, 1, int.MaxValue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IAggregateEvent<Guid>[] { });

            // act
            GiftCard actual = await _sut.FindGiftCardAsync(id);

            // assert
            Assert.Null(actual);
        }

        [Theory, AutoData]
        public async Task FindAggregateAsync_NoSnapshot_HappyPath(Guid id)
        {
            // arrange
            _eventStore
                .Setup(x => x.GetEventsAsync(id, 1, int.MaxValue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IAggregateEvent<Guid>[] {
                    new GiftCardCreated(id, DateTime.UtcNow.AddDays(-2), 100),
                    new GiftCardDebited(id, 2, DateTime.UtcNow.AddDays(-1), 30)
                });

            // act
            GiftCard actual = await _sut.FindGiftCardAsync(id);

            // assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Version);
            Assert.Null(actual.Snapshot);
            Assert.Equal(70, actual.Balance);
        }

        [Theory, AutoData]
        public async Task FindAggregateAsync_IgnoreSnapshot_HappyPath(Guid id)
        {
            _eventStore
                .Setup(x => x.GetEventsAsync(id, 1, int.MaxValue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IAggregateEvent<Guid>[] {
                    new GiftCardCreated(id, DateTime.UtcNow.AddDays(-2), 100),
                    new GiftCardDebited(id, 2, DateTime.UtcNow.AddDays(-1), 30)
                });

            // act
            GiftCard actual = await _sut.FindGiftCardAsync(id, ignoreSnapshot: true);

            // assert
            Assert.NotNull(actual);
            Assert.Equal(70, actual.Balance);
            _snapshotStore.Verify(x => x.FindLastSnapshotAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory, AutoData]
        public async Task FindAggregateAsync_WithSnapshot_HappyPath(Guid id, int snapshotVersion)
        {
            // arrange
            _snapshotStore
                .Setup(x => x.FindLastSnapshotAsync(id, int.MaxValue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GiftCardSnapshot(id, snapshotVersion, 100));
            _eventStore
                .Setup(x => x.GetEventsAsync(id, snapshotVersion + 1, int.MaxValue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IAggregateEvent<Guid>[] {
                    new GiftCardDebited(id, snapshotVersion + 1, DateTime.UtcNow.AddDays(-1), 30)
                });

            // act
            GiftCard actual = await _sut.FindGiftCardAsync(id);

            // assert
            Assert.NotNull(actual);
            Assert.Equal(snapshotVersion + 1, actual.Version);
            Assert.NotNull(actual.Snapshot);
            Assert.Equal(70, actual.Balance);
        }

        [Theory, AutoData]
        public async Task FindAggregateAsync_WithVersion_WithSnapshot_HappyPath(Guid id, int version)
        {
            // arrange
            _snapshotStore
                .Setup(x => x.FindLastSnapshotAsync(id, version, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GiftCardSnapshot(id, version - 2, 100));
            _eventStore
                .Setup(x => x.GetEventsAsync(id, version - 1, version, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IAggregateEvent<Guid>[] {
                    new GiftCardDebited(id, version - 1, DateTime.UtcNow.AddDays(-1), 30),
                    new GiftCardDebited(id, version, DateTime.UtcNow, 30)
                });

            // act
            GiftCard actual = await _sut.FindGiftCardAsync(id, version: version);

            // assert
            Assert.NotNull(actual);
            Assert.Equal(40, actual.Balance);
        }

        [Theory, AutoData]
        public async Task FindAggregateAsync_WithVersion_WithoutSnapshot_HappyPath(Guid id)
        {
            // arrange
            _eventStore
                .Setup(x => x.GetEventsAsync(id, 1, 3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IAggregateEvent<Guid>[] {
                    new GiftCardCreated(id, DateTime.UtcNow.AddDays(-2), 100),
                    new GiftCardDebited(id, 2, DateTime.UtcNow.AddDays(-1), 30),
                    new GiftCardDebited(id, 3, DateTime.UtcNow, 30)
                });

            // act
            GiftCard actual = await _sut.FindGiftCardAsync(id, version: 3);

            // assert
            Assert.NotNull(actual);
            Assert.Null(actual.Snapshot);
            Assert.Equal(40, actual.Balance);
        }
    }
}
