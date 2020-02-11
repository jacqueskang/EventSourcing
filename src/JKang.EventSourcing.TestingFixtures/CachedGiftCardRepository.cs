using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Snapshotting.Persistence;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.TestingFixtures
{
    public class CachedGiftCardRepository : CachedAggregateRepository<GiftCard, Guid>, IGiftCardRepository
    {
        public CachedGiftCardRepository(
            IEventStore<GiftCard, Guid> eventStore,
            ISnapshotStore<GiftCard, Guid> snapshotStore,
            IDistributedCache cache)
            : base(eventStore, snapshotStore, cache, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(10)
            })
        { }

        public Task SaveGiftCardAsync(GiftCard giftCard) => SaveAggregateAsync(giftCard);

        public Task<GiftCard> FindGiftCardAsync(Guid id) => FindAggregateAsync(id);

        public Task<Guid[]> GetGiftCardIdsAsync() => GetAggregateIdsAsync();
    }
}
