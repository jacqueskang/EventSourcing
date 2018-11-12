using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Samples.Domain;
using System;
using System.Threading.Tasks;

namespace Samples.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAccountRepository _store;

        public IndexModel(IAccountRepository store)
        {
            _store = store;
        }

        public Guid[] AccountIds { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            AccountIds = await _store.GetAccountIdsAsync();
            return Page();
        }
    }
}
