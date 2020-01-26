using JKang.EventSourcing.TestingFixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.TestingWebApp.Pages
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
