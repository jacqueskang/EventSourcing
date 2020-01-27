[![Build Status](https://travis-ci.com/jacqueskang/EventSourcing.svg?branch=develop)](https://travis-ci.com/jacqueskang/EventSourcing)

# EventSourcing

A .NET Core event sourcing framework.

Easy to be integrated in ASP.NET Core web application, Lambda function or Azure function.

Support various of event store:
 - in text file (one text file per aggregate)
 - in relational database such as SQL Server, MySQL, etc. using EF Core  (See [EF Core setup instructions](doc/EfCoreSetup.md))
 - in AWS DynamoDB (See [DynamoDB setup instructions](doc/DynamoDBSetup.md))
 - in Azure CosmosDB (See [CosmosDB setup instructions](doc/CosmosDBSetup.md))

## NuGet packages
 - JKang.EventSourcing [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.svg)](https://badge.fury.io/nu/JKang.EventSourcing)
 - JKang.EventSourcing.Persistence.FileSystem [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.FileSystem.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.FileSystem)
 - JKang.EventSourcing.Persistence.EfCore [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.EfCore.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.EfCore)
 - JKang.EventSourcing.Persistence.DynamoDB [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.DynamoDB.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.DynamoDB)
 - JKang.EventSourcing.Persistence.CosmosDB [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.CosmosDB.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.CosmosDB)

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

In order to use built-in event store, please make sure event can be properly serialized using [Json.NET](https://www.newtonsoft.com/json).

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
 2. Have a public constructor with signature `MyAggregate(TKey id, IEnumerable<IAggregateEvent<TKey>> savedEvents)` (which is used for recompiling from history events)

You can optionally inherit from the abstract class `Aggregate<TKey>` provided by the framework.

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

By definition of Event Sourcing, persisting an aggregate insists on persisting all historical events.

You can optionally override the abstract class `AggregateRepository<TAggregate, TAggregateKey>`.

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

### Step 4 - Register your repository interface and configure event store in dependency injection framework

```csharp
    services
        .AddScoped<IGiftCardRepository, GiftCardRepository>()
        .AddEventSourcing(builder =>
        {
            builder.UseTextFileEventStore<GiftCard, Guid>(x =>
            {
                x.Folder = "C:\\Temp\\EventSourcing\\GiftCards";
            });
        });
```
Note: It's possible to configure multiple aggregate types for different event stores

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
