using JKang.EventSourcing.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JKang.EventSourcing.TestingFixtures
{
    public class GiftCardRepository : AggregateRepository<GiftCard, Guid>, IGiftCardRepository
    {
        public GiftCardRepository(IEventStore<GiftCard, Guid> eventStore)
            : base(eventStore)
        { }

        public Task SaveGiftCardAsync(GiftCard giftCard) => SaveAggregateAsync(giftCard);

        public Task<GiftCard> FindGiftCardAsync(Guid id) => FindAggregateAsync(id);

        public Task<Guid[]> GetGiftCardIdsAsync() => GetAggregateIdsAsync();
    }
}
