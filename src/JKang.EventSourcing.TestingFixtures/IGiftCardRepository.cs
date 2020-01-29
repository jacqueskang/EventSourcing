using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.TestingFixtures
{
    public interface IGiftCardRepository
    {
        Task SaveGiftCardAsync(GiftCard giftCard);
        Task<GiftCard> FindGiftCardAsync(Guid id, bool useSnapshot);
        Task<Guid[]> GetGiftCardIdsAsync();
    }
}
