using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Samples.Domain;
using System.Threading.Tasks;

namespace Samples.WebApp.Pages.GiftCards
{
    public class CreateModel : PageModel
    {
        private readonly IGiftCardRepository _repository;

        [BindProperty]
        public decimal InitialCredit { get; set; } = 100;

        public CreateModel(IGiftCardRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var giftCard = new GiftCard(InitialCredit);
            await _repository.SaveGiftCardAsync(giftCard);
            return RedirectToPage("/GiftCards/Details", new { id = giftCard.Id });
        }
    }
}