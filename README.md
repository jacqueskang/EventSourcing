[![Build Status](https://travis-ci.org/jacqueskang/EventSourcing.svg?branch=develop)](https://travis-ci.org/jacqueskang/EventSourcing)

# EventSourcing

A .NET Core event sourcing framework.

Easy to integrate in ASP.NET Core project to persist event-sourced domain entities in file system (one text file per aggregate) or in database (using EF Core, one table per aggregate type).

## NuGet packages
 - [JKang.EventSourcing](https://www.nuget.org/packages/JKang.EventSourcing/)

## Quick Start:

Let's implement a simple gift card management system with the following use cases:
 * Create gift cards with initial credit
 * Debit the gift card specifying amount
   * Overpaying is not allowed
   * Payment history should be persisted

I'm adopting *DDD (Domain Driven Design)* approach and implement the *GiftCard* entity as an **Rich Domain Aggregate** which encapsulates/protects its internal data/state, and contains itself business logics ensuring data integrity.

**Notes**:
 - To improve readability and I'm omitting some necessary codes (e.g. Json constructor). The complete sample project can be found in the solution.

### Step 1 - Define events

2 events are needed for our use cases: 

```csharp
    public sealed class GiftCardCreated : AggregateCreatedEvent
    {
        public GiftCardCreated(Guid aggregateId, decimal initialCredit)
            : base(aggregateId)
        {
            InitialCredit = initialCredit;
        }

        public decimal InitialCredit { get; }
    }
```

```csharp
    public class GiftCardDebited : AggregateEvent
    {
        public GiftCardDebited(Guid aggregateId, int aggregateVersion, decimal amount)
            : base(aggregateId, aggregateVersion)
        {
            Amount = amount;
        }

        public decimal Amount { get; }
    }
```

### Step 2 - Implement domain aggregate

```csharp
    public class GiftCard : Aggregate
    {
        /// <summary>
        /// Constructor for an new aggregate
        /// </summary>
        public GiftCard(decimal initialCredit)
            : this(Guid.NewGuid(), initialCredit)
        { }

        private GiftCard(Guid id, decimal initialCredit)
            : base(id, new GiftCardCreated(id, initialCredit))
        { }

        /// <summary>
        /// Constructor for rehydrate the aggregate from historical events
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <param name="savedEvents">Historical events</param>
        public GiftCard(Guid id, IEnumerable<AggregateEvent> savedEvents)
            : base(id, savedEvents)
        { }

        public decimal Balance { get; private set; }

        public void Debit(decimal amout)
        {
            ReceiveEvent(new GiftCardDebited(Id, NextVersion, amout));
        }

        protected override void ApplyEvent(AggregateEvent @event)
        {
            if (@event is GiftCardCreated created)
            {
                Balance = created.InitialCredit;
            }
            else if (@event is GiftCardDebited debited)
            {
                if (Balance < debited.Amount)
                {
                    throw new InvalidOperationException("Not enough credit");
                }
                Balance -= debited.Amount;
            }
        }
    }
```

### Step 3 - Implement repository

```csharp
    public interface IGiftCardRepository
    {
        Task SaveGiftCardAsync(GiftCard giftCard);
        Task<GiftCard> FindGiftCardAsync(Guid id);
    }
```
    
```csharp
    public class GiftCardRepository : AggregateRepository<GiftCard>, IGiftCardRepository
    {
        public GiftCardRepository(IEventStore<GiftCard> eventStore)
            : base(eventStore)
        { }

        public Task SaveGiftCardAsync(GiftCard giftCard) => SaveAggregateAsync(giftCard);

        public Task<GiftCard> FindGiftCardAsync(Guid id) => FindAggregateAsync(id);
    }
```

### Step 4 - Setup dependency injection and configure event store

```csharp
    services
        .AddScoped<IGiftCardRepository, GiftCardRepository>()
```

Note: It's possible to configure different event store for each aggregate type:

* File system event store

```csharp
    services
        .AddEventSourcing()
        .UseTextFileEventStore<GiftCard>(x =>
        {
            x.Folder = "C:\\Temp\\EventSourcing\\GiftCards";
        })
```

* Database event store (using EF Core)

```csharp
    public class SampleDbContext : DbContext, IEventSourcingDbContext<GiftCard>
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        { }

        public DbSet<EventEntity> GiftCardEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventEntityConfiguration());
        }

        DbSet<EventEntity> IEventSourcingDbContext<GiftCard>.GetDbSet()
        {
            return GiftCardEvents;
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
        .UseDbEventStore<SampleDbContext, GiftCard>();
```

### Now it's possible to resolve IGiftCardRepository from DI to create and use gift cards.


```csharp
    // create a new gift card with initial credit 100
    var giftCard = new GiftCard(100);

    // persist the gift card
    await _repository.SaveGiftCardAsync(giftCard);

    // rehydrate the giftcard
    giftCard = await _repository.FindGiftCardAsync(giftCard.Id);

    // payments
    giftCard.Debit(40); // ==> balance: 60
    giftCard.Debit(50); // ==> balance: 10
    giftCard.Debit(20); // ==> invalid operation exception
```

__Please feel free to download, fork and/or provide any feedback!__
