using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Samples.Domain;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Samples.WebApp.Pages.Accounts
{
    public class CreateModel : PageModel
    {
        private readonly IAccountRepository _repository;

        [Required]
        [BindProperty]
        public string Name { get; set; }

        public CreateModel(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var account = new Account(Name);
            await _repository.SaveAccountAsync(account);
            return RedirectToPage("/Accounts/Details", new { id = account.Id });
        }
    }
}