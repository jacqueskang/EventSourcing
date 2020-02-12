using JKang.EventSourcing.TestingFixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.TestingWebApp.Pages.GiftCards
{
    public class DetailsModel : PageModel
    {
        private readonly IGiftCardRepository _repository;

        public DetailsModel(IGiftCardRepository repository)
        {
            _repository = repository;
        }

        public GiftCard GiftCard { get; private set; }

        [TempData]
        public string Error { get; set; }

        [BindProperty]
        public decimal Amount { get; set; } = 30;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            GiftCard = await _repository.FindGiftCardAsync(id)
                ?? throw new InvalidOperationException("Gift card not found");

            return Page();
        }

        public async Task<IActionResult> OnPostDebitAsync(Guid id)
        {
            try
            {
                GiftCard = await _repository.FindGiftCardAsync(id)
                    ?? throw new InvalidOperationException("Gift card not found");

                GiftCard.Debit(Amount);
                await _repository.SaveGiftCardAsync(GiftCard);
            }
            catch (InvalidOperationException ex)
            {
                Error = ex.Message;
            }
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostTakeSnapshot(Guid id)
        {
            GiftCard giftCard = await _repository.FindGiftCardAsync(id)
                ?? throw new InvalidOperationException("Gift card not found");

            giftCard.TakeSnapshot();
            await _repository.SaveGiftCardAsync(giftCard);
            return RedirectToPage(new { id });
        }
    }
}