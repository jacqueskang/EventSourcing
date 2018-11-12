using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Samples.Domain;

namespace Samples.WebApp.Pages.Accounts
{
    public class DetailsModel : PageModel
    {
        private readonly IAccountRepository _repository;

        public DetailsModel(IAccountRepository repository)
        {
            _repository = repository;
        }

        public Account Account { get; private set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Account = await _repository.FindAccountAsync(id);
            if (Account == null)
            {
                TempData["ErrorMessage"] = $"Account '{id}' not found.";
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}