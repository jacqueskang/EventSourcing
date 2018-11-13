[![Build Status](https://travis-ci.org/jacqueskang/EventSourcing.svg?branch=develop)](https://travis-ci.org/jacqueskang/EventSourcing)

# EventSourcing

A .NET Core event sourcing framework.

Easy to integrate in ASP.NET Core project to persist event-sourced domain entities in file system (one text file per aggregate) or in database (using EF Core, one table per aggregate type).

## NuGet packages
 - [JKang.EventSourcing](https://www.nuget.org/packages/JKang.EventSourcing/)

## Quick Start:

Let's implement a really simple banking account management system, with which we can
 * Create an account
 * Credit the account
 * Debit the account

I'm adopting *DDD (Domain Driven Design)* approach and implement **Account** as an **Rich Domain Aggregate** which encapsulates/protects its internal data/state, and contains itself business logics ensuring data integrity.

**Notes**:
 - To improve readability and I'm omitting some necessary codes (e.g. Json constructor). The complete sample project can be found in the solution.

### Step 1 - Define events

3 events are needed for our use cases: 

```csharp
    public sealed class AccountCreated : AggregateCreatedEvent
    {
        public AccountCreated(Guid aggregateId, string name)
            : base(aggregateId)
        {
            Name = name;
        }

        public string Name { get; }
    }
```

```csharp
    public class AccountCredited : AggregateEvent
    {
        public AccountCredited(Guid aggregateId, int aggregateVersion, decimal amount, string reason)
            : base(aggregateId, aggregateVersion)
        {
            Amount = amount;
            Reason = reason;
        }

        public decimal Amount { get; }
        public string Reason { get; }
    }
```

```csharp
    public class AccountDebited : AggregateEvent
    {
        public AccountDebited(Guid aggregateId, int aggregateVersion, decimal amount, string reason)
            : base(aggregateId, aggregateVersion)
        {
            Amount = amount;
            Reason = reason;
        }

        public decimal Amount { get; }
        public string Reason { get; }
    }
```

### Step 2 - Implement domain aggregate

```csharp
    public class Account : Aggregate
    {
        /// <summary>
        /// Constructor for creating an new account
        /// </summary>
        /// <param name="name">Account name</param>
        public Account(string name)
            : this(Guid.NewGuid(), name)
        { }

        private Account(Guid id, string name)
            : base(id, new AccountCreated(id, name))
        { }

        /// <summary>
        /// Constructor for rebuilding account from historical events
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <param name="savedEvents">Historical events</param>
        public Account(Guid id, IEnumerable<AggregateEvent> savedEvents)
            : base(id, savedEvents)
        { }

        public string Name { get; private set; }

        public decimal Balance { get; private set; }

        public void Credit(decimal amout, string reason)
        {
            ReceiveEvent(new AccountCredited(Id, NextVersion, amout, reason));
        }

        public void Debit(decimal amout, string reason)
        {
            ReceiveEvent(new AccountDebited(Id, NextVersion, amout, reason));
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
    
    public class AccountRepository : AggregateRepository<Account>, IAccountRepository
    {
        public AccountRepository(IEventStore eventStore)
            : base(eventStore)
        { }

        public Task SaveAccountAsync(Account account) => SaveAggregateAsync(account);

        public Task<Account> FindAccountAsync(Guid id) => FindAggregateAsync(id);
    }
```

### Step 4 - Setup dependency injection and configure event store

```csharp
    services
        .AddScoped<IAccountRepository, AccountRepository>();
```

It's possible to use different event store for each type of aggregate:

1. File system event store
```csharp
    services
        .AddEventSourcing()
        .UseTextFileEventStore<Account>(x =>
        {
            x.Folder = "C:\\Temp\\EventSourcing\\Accounts";
        });
```

2. Database event store (using EF Core)
```csharp
    public class SampleDbContext : DbContext, IEventSourcingDbContext<Account>
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        { }

        public DbSet<EventEntity> AccountEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventEntityConfiguration());
        }

        DbSet<EventEntity> IEventSourcingDbContext<Account>.GetDbSet()
        {
            return AccountEvents;
        }
    }
```

```csharp
    services
        .AddDbContext<SampleDbContext>(x =>
        {
            x.UseInMemoryDatabase("eventstore");
        });

    services
        .AddEventSourcing()
        .UseDbEventStore<SampleDbContext, Account>();
```

### Now it's possible to resolve IAccountRepository from DI to create and manage accounts.

* Create and persist a new account

```csharp
    var account = new Account(Name);
    await _repository.SaveAccountAsync(account);
```

* Rehydrate account, debit/credit and persist again
```csharp
    Account account = await _repository.FindAccountAsync(id);
    account.Credit(100, "Initial saving");
    account.Debit(50, "Credit card");
    account.Debit(60, "Credit card"); // ==> invalid operation exception
```

__Please feel free to download, fork and/or provide any feedback!__
