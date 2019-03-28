using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Samples.Domain;
using System;
using System.Threading.Tasks;

namespace Samples.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IGiftCardRepository _repository;

        public IndexModel(IGiftCardRepository repository)
        {
            _repository = repository;
        }

        public Guid[] GiftCardIds { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            GiftCardIds = await _repository.GetGiftCardIdsAsync();
            return Page();
        }
    }
}
