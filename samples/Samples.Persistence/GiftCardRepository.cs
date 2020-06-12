using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Snapshotting.Persistence;
using Samples.Domain;
using System;
using System.Threading.Tasks;

namespace Samples.Persistence
{
    public class GiftCardRepository : AggregateRepository<GiftCard, Guid>, IGiftCardRepository
    {
        public GiftCardRepository(
            IEventStore<GiftCard, Guid> eventStore,
            ISnapshotStore<GiftCard, Guid> snapshotStore)
            : base(eventStore, snapshotStore)
        { }

        public Task SaveGiftCardAsync(GiftCard giftCard) =>
            SaveAggregateAsync(giftCard);

        public Task<GiftCard> FindGiftCardAsync(Guid id) =>
            FindAggregateAsync(id, true);

        public Task<Guid[]> GetGiftCardIdsAsync() =>
            GetAggregateIdsAsync();
    }
}
