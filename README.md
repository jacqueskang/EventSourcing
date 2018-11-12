[![Build Status](https://travis-ci.org/jacqueskang/EventSourcing.svg?branch=develop)](https://travis-ci.org/jacqueskang/EventSourcing)

# EventSourcing

A .NET Core event sourcing framework.

Easy to integrate in ASP.NET Core project to persist event-sourced domain entities in different storage: Azure blob, AWS cloud object storage.

## Usage
TODO

## Downloads
TODO

## Quick Start:

Let's implement a really simple banking account management system, with which we can
 * Create an account
 * Credit the account
 * Debit the account

I'm adopting *DDD (Domain Driven Design)* approach and implement **Account** as an **Rich Domain Entity** which encapsulates/protects its internal data/state, and contains itself business logics ensuring data integrity.

**Notes**:
 - To improve readability and I'm omitting some necessary codes (e.g. Json constructor). The complete sample project can be found in the solution.

### Step 1 - Define events

3 events are needed for our use cases: 
 * `AccountCreated`
 * `AccountCredited`
 * `AccountDebited`

```csharp
    public sealed class AccountCreated : Event
    {
        public AccountCreated(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class AccountCredited : Event
    {
        public AccountCredited(decimal amount, string reason)
        {
            Amount = amount;
            Reason = reason;
        }

        public decimal Amount { get; }
        public string Reason { get; }
    }

    public class AccountDebited : Event
    {
        public AccountDebited(decimal amount, string reason)
        {
            Amount = amount;
            Reason = reason;
        }

        public decimal Amount { get; }
        public string Reason { get; }
    }
```

### Step 2 - Implement domain entity

```csharp
    public class Account : EventSourcedEntity
    {
        /// <summary>
        /// Constructor for creating an new account
        /// </summary>
        /// <param name="name">Account name</param>
        public Account(string name)
            : base(Guid.NewGuid(), new AccountCreated(name))
        { }

        /// <summary>
        /// Constructor for rebuilding account from historical events
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <param name="history">Historical events</param>
        public Account(Guid id, IEnumerable<IEvent> history)
            : base(id, history)
        { }

        public string Name { get; private set; }
        public decimal Balance { get; private set; }

        public void Credit(decimal amout, string reason)
        {
            ReceiveEvent(new AccountCredited(amout, reason));
        }

        public void Debit(decimal amout, string reason)
        {
            ReceiveEvent(new AccountDebited(amout, reason));
        }

        protected override void ProcessEvent(IEvent @event)
        {
            if (@event is AccountCreated accountCreated)
            {
                Name = accountCreated.Name;
            }
            else if (@event is AccountCredited accountCredited)
            {
                Balance += accountCredited.Amount;
            }
            else if (@event is AccountDebited accountDebited)
            {
                if (Balance >= accountDebited.Amount)
                {
                    Balance -= accountDebited.Amount;
                }
                else
                {
                    throw new InvalidOperationException("Not enough credit");
                }
            }
        }
    }
```

### Step 3 - Implement repository

```csharp
    public interface IAccountRepository
    {
        Task SaveAccountAsync(Account account);
        Task<Account> FindAccountAsync(Guid id);
    }
	
	public class AccountRepository : EventSourcedEntityRepository<Account>, IAccountRepository
    {
        public AccountRepository(IEventStore eventStore)
            : base(eventStore)
        { }

        public Task SaveAccountAsync(Account account)
        {
            return SaveEntityAsync(account);
        }

        public Task<Account> FindAccountAsync(Guid id)
        {
            return FindEntityAsync(id);
        }
    }
```

### Step 4 - Setup dependency injection

```csharp
    services
        .AddScoped<IAccountRepository, AccountRepository>();

    services
        .AddEventSourcing()
        .UseFileSystemBinaryStore(x => x.Folder = "C:\\Temp\\EventSourcing");
```

### Step 5 - Implement UI

#### Create account

```asp
	<form method="post">
		<div class="form-group">
			<label asp-for="Name"></label>
			<input asp-for="Name" class="form-control" />
			<span class="text-danger" asp-validation-for="Name"></span>
		</div>

		<input type="submit" class="btn btn-primary" value="Create" />
	</form>
```

```csharp
    public class CreateModel : PageModel
    {
        private readonly IAccountRepository _repository;

        [Required]
        [BindProperty]
        public string Name { get; set; }

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
```

#### View account
```asp
	<dl class="dl-horizontal">
		<dt>Id</dt>
		<dd>@Model.Account.Id</dd>

		<dt>Name</dt>
		<dd>@Model.Account.Name</dd>

		<dt>Balance</dt>
		<dd>@Model.Account.Balance.ToString("0.00") €</dd>
	</dl>

	<hr />

	<h3>History</h3>
	<ol>
		@foreach (var @e in Model.Account.Events)
		{
			<li>@e</li>
		}
	</ol>
```

```csharp
    public class DetailsModel : PageModel
    {
        private readonly IAccountRepository _repository;

        public Account Account { get; private set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Account = await _repository.FindAccountAsync(id)
                ?? throw new InvalidOperationException("Account not found");

            return Page();
        }
    }
```

#### Debit/Credit account
```asp
	<h3>Operations</h3>

	<form method="post" class="form">
		<div class="text-danger" asp-validation-summary="ModelOnly"></div>

		<div class="form-group">
			<label asp-for="Amount"></label>
			<input asp-for="Amount" class="form-control" />
			<span class="text-danger" asp-validation-for="Amount"></span>
		</div>

		<div class="form-group">
			<label asp-for="Reason"></label>
			<input asp-for="Reason" class="form-control" />
			<span class="text-danger" asp-validation-for="Reason"></span>
		</div>

		<input class="btn btn-success" type="submit" asp-page-handler="Credit" value="Credit" asp-route-id="@Model.Account.Id" />
		<input class="btn btn-danger" type="submit" asp-page-handler="Debit" value="Debit" asp-route-id="@Model.Account.Id" />
	</form>
```

```csharp
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
```

__Please feel free to download, fork and/or provide any feedback!__
