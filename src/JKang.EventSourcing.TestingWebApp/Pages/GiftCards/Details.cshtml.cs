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

        [BindProperty]
        public decimal Amount { get; set; } = 30;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            GiftCard = await _repository.FindGiftCardAsync(id, true)
                ?? throw new InvalidOperationException("Gift card not found");

            return Page();
        }

        public async Task<IActionResult> OnPostDebitAsync(Guid id)
        {
            GiftCard = await _repository.FindGiftCardAsync(id, true)
                ?? throw new InvalidOperationException("Gift card not found");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                GiftCard.Debit(Amount);
                await _repository.SaveGiftCardAsync(GiftCard);
                return RedirectToPage(new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostTakeSnapshot(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}