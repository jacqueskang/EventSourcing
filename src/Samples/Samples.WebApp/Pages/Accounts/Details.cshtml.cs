using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [BindProperty]
        public decimal Amount { get; set; }

        [Required]
        [BindProperty]
        public string Reason { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Account = await _repository.FindAccountAsync(id)
                ?? throw new InvalidOperationException("Account not found");

            return Page();
        }

        public async Task<IActionResult> OnPostCreditAsync(Guid id)
        {
            Account = await _repository.FindAccountAsync(id)
                ?? throw new InvalidOperationException("Account not found");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Account.Credit(Amount, Reason);
                await _repository.SaveAccountAsync(Account);
                return RedirectToPage(new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }


        public async Task<IActionResult> OnPostDebitAsync(Guid id)
        {
            Account = await _repository.FindAccountAsync(id)
                ?? throw new InvalidOperationException("Account not found");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Account.Debit(Amount, Reason);
                await _repository.SaveAccountAsync(Account);
                return RedirectToPage(new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }
    }
}