using JKang.EventSourcing.TestingFixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace JKang.EventSourcing.TestingWebApp.Pages.GiftCards
{
    public class PreviousVersionModel : PageModel
    {
        private readonly IGiftCardRepository _repository;

        public PreviousVersionModel(IGiftCardRepository repository)
        {
            _repository = repository;
        }

        public int MaxVersion { get; private set; }
        public bool IgnoreSnapshot { get; private set; }
        public GiftCard GiftCard { get; private set; }

        public async Task<IActionResult> OnGetAsync(Guid id, int maxVersion, string ignoreSnapshot = "off", int version = -1)
        {
            MaxVersion = maxVersion;
            IgnoreSnapshot = ignoreSnapshot == "on";
            GiftCard = await _repository.FindGiftCardAsync(id, IgnoreSnapshot, version)
                ?? throw new InvalidOperationException("Gift card not found");

            return Page();
        }
    }
}