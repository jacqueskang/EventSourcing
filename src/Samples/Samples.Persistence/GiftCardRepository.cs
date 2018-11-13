using JKang.EventSourcing.Persistence;
using Samples.Domain;
using System;
using System.Threading.Tasks;

namespace Samples.Persistence
{
    public class GiftCardRepository : AggregateRepository<GiftCard>, IGiftCardRepository
    {
        public GiftCardRepository(IEventStore<GiftCard> eventStore)
            : base(eventStore)
        { }

        public Task SaveGiftCardAsync(GiftCard giftCard) => SaveAggregateAsync(giftCard);

        public Task<GiftCard> FindGiftCardAsync(Guid id) => FindAggregateAsync(id);

        public Task<Guid[]> GetGiftCardIdsAsync() => GetAggregateIdsAsync();
    }
}
