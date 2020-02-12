using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Snapshotting.Persistence;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.TestingFixtures
{
    public class GiftCardRepository : AggregateRepository<GiftCard, Guid>, IGiftCardRepository
    {
        public GiftCardRepository(
            IEventStore<GiftCard, Guid> eventStore,
            ISnapshotStore<GiftCard, Guid> snapshotStore)
            : base(eventStore, snapshotStore)
        { }

        public Task SaveGiftCardAsync(GiftCard giftCard) => SaveAggregateAsync(giftCard);

        public Task<GiftCard> FindGiftCardAsync(Guid id,
            bool ignoreSnapshot = false,
            int version = -1) =>
            FindAggregateAsync(id, ignoreSnapshot, version);

        public Task<Guid[]> GetGiftCardIdsAsync() => GetAggregateIdsAsync();
    }
}
