[![Build Status](https://travis-ci.com/jacqueskang/EventSourcing.svg?branch=develop)](https://travis-ci.com/jacqueskang/EventSourcing)

# EventSourcing

A .NET Core event sourcing framework.

Easy to integrate in ASP.NET Core project to persist event-sourced domain entities in:
 - file system (one text file per aggregate)
 - in relational database (using EF Core, one table per aggregate type)
 - in AWS DynamoDB (See [DynamoDB setup instructions](doc/DynamoDBSetup.md))

## NuGet packages
 - JKang.EventSourcing [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.svg)](https://badge.fury.io/nu/JKang.EventSourcing)
 - JKang.EventSourcing.Persistence.FileSystem [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.FileSystem.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.FileSystem)
 - JKang.EventSourcing.Persistence.EfCore [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.EfCore.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.EfCore)
 - JKang.EventSourcing.Persistence.DynamoDB [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.DynamoDB.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.DynamoDB)

## Quick Start:

Let's implement a simple gift card management system with the following use cases:
 * Create gift cards with initial credit
 * Debit the gift card specifying amount
   * Overpaying is not allowed
   * Payment history should be persisted

>
> A demostration web application can be find [__HERE__](https://jkang-event-sourcing.azurewebsites.net/)
>

I'm adopting *DDD (Domain Driven Design)* approach and implement the *GiftCard* entity as an **Rich Domain Aggregate** which encapsulates/protects its internal data/state, and contains itself business logics ensuring data integrity.

### Step 1 - Define events

The minimum requirement for an user defined event is to implement the following interface:
```csharp
    public interface IAggregateEvent<TAggregateKey>
    {
        // ID of domain aggregate
        TAggregateKey AggregateId { get; }

        // Version of domain aggregate after event occurred
        int AggregateVersion { get; }

        // Timestamp of event
        DateTime Timestamp { get; }
    }
```
It's recommended that to implement an event in an **immutable** way.

Event must be serializable. The framework uses Json.NET by default but you can customize the serialization by providing your own implementation of `IObjectSerializer` interface. (e.g., with Protobuf)

You can optionally inherit from the abstract classes `AggregateEvent<TAggregateKey>` or `AggregateCreatedEvent<TAggregateKey>` provided by the framework to save several lines of code.


For our use cases I'm defining 2 events as following: 

```csharp
    public sealed class GiftCardCreated : AggregateCreatedEvent<Guid>
    {
        public GiftCardCreated(Guid aggregateId, DateTime timestamp, decimal initialCredit)
            : base(aggregateId, timestamp)
        {
            InitialCredit = initialCredit;
        }

        public decimal InitialCredit { get; }
    }
```

```csharp
    public class GiftCardDebited : AggregateEvent<Guid>
    {
        public GiftCardDebited(Guid aggregateId, int aggregateVersion, DateTime timestamp, decimal amount)
            : base(aggregateId, aggregateVersion, timestamp)
        {
            Amount = amount;
        }

        public decimal Amount { get; }
    }
```

### Step 2 - Implement domain aggregate

The minimum requirements of a domain aggregate are:
 1. Implement `IAggregate<TKey>` interface
 2. Have a public constructor with signature: `public YourCustomAggregate(TKey id, IEnumerable<IAggregateEvent<TKey>> savedEvents)`

You can inherit from the abstract class `Aggregate<TKey>` provided by the framework.

```csharp
    public class GiftCard : Aggregate<Guid>
    {
        /// <summary>
        /// Constructor for an new aggregate
        /// </summary>
        public GiftCard(decimal initialCredit)
            : base(new GiftCardCreated(Guid.NewGuid(), DateTime.UtcNow, initialCredit))
        { }

        /// <summary>
        /// Constructor for rehydrate the aggregate from historical events
        /// </summary>
        public GiftCard(Guid id, IEnumerable<IAggregateEvent<Guid>> savedEvents)
            : base(id, savedEvents)
        { }

        public decimal Balance { get; private set; }

        public void Debit(decimal amout)
            => ReceiveEvent(new GiftCardDebited(Id, GetNextVersion(), DateTime.UtcNow, amout));

        protected override void ApplyEvent(IAggregateEvent<Guid> @event)
        {
            if (@event is GiftCardCreated created)
            {
                Balance = created.InitialCredit;
            }
            else if (@event is GiftCardDebited debited)
            {
                if (debited.Amount < 0)
                {
                    throw new InvalidOperationException("Negative debit amout is not allowed.");
                }

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

By definition of Event Sourcing, persisting an aggregate insists on persisting all historical events which is done by IEventStore implementation.
The framework provides 2 IEventStore implementations (TextFileEventStore & DatabaseEventStore) and an abstract class `AggregateRepository<TAggregate, TAggregateKey>` to help implementing your aggregate repository.

```csharp
    public interface IGiftCardRepository
    {
        Task SaveGiftCardAsync(GiftCard giftCard);
        Task<GiftCard> FindGiftCardAsync(Guid id);
    }
```
    
```csharp
    public class GiftCardRepository : AggregateRepository<GiftCard, Guid>, IGiftCardRepository
    {
        public GiftCardRepository(IEventStore<GiftCard, Guid> eventStore)
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
        .AddEventSourcing(builder =>
        {
            builder.UseTextFileEventStore<GiftCard, Guid>(x =>
            {
                x.Folder = "C:\\Temp\\EventSourcing\\GiftCards";
            });
        });
```

* Database event store (using EF Core)

```csharp
    public class SampleDbContext : DbContext, IEventSourcingDbContext<GiftCard, Guid>
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        { }

        public DbSet<EventEntity<Guid>> GiftCardEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfiguration(new EventEntityConfiguration<Guid>());

        DbSet<EventEntity<Guid>> IEventSourcingDbContext<GiftCard, Guid>.GetDbSet()
            => GiftCardEvents;
    }
```

```csharp
    services
        .AddDbContext<SampleDbContext>(x =>
        {
            x.UseInMemoryDatabase("eventstore");
        })
        .AddEventSourcing(builder =>
        {
            builder.UseDbEventStore<SampleDbContext, GiftCard, Guid>();
        })
        ;
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
