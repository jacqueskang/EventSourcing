using JKang.EventSourcing.Persistence;
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

        public Task<GiftCard> FindGiftCardAsync(Guid id, bool useSnapshot) => FindAggregateAsync(id, useSnapshot);

        public Task<Guid[]> GetGiftCardIdsAsync() => GetAggregateIdsAsync();
    }
}
