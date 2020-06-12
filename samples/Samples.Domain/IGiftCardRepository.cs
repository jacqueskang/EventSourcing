using System;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.Domain
{
    public interface IGiftCardRepository
    {
        Task SaveGiftCardAsync(GiftCard giftCard);
        Task<GiftCard> FindGiftCardAsync(Guid id);
        Task<Guid[]> GetGiftCardIdsAsync();
    }
}
