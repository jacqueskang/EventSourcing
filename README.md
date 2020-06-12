[![Build Status](https://dev.azure.com/jacques-kang/EventSourcing/_apis/build/status/jacqueskang-eventsourcing-ci?branchName=develop)](https://dev.azure.com/jacques-kang/EventSourcing/_build/latest?definitionId=11&branchName=develop)
# EventSourcing

A .NET Core event sourcing framework.

Easy to be integrated in ASP.NET Core web application, Lambda function or Azure function.

Support various of event store:
 - in file system as plain text file (see [File system setup instructions](doc/FileSystemSetup.md))
 - in AWS DynamoDB (see [DynamoDB setup instructions](doc/DynamoDBSetup.md))
 - in Azure CosmosDB (see [CosmosDB setup instructions](doc/CosmosDBSetup.md))
 - in any relational database supported by EF Core, e.g., Microsoft SQL Server,MySQL, etc. (see [EF Core setup instructions](doc/EfCoreSetup.md))

## NuGet packages
 - JKang.EventSourcing [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.svg)](https://badge.fury.io/nu/JKang.EventSourcing)
 - JKang.EventSourcing.Persistence.FileSystem [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.FileSystem.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.FileSystem)
 - JKang.EventSourcing.Persistence.EfCore [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.EfCore.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.EfCore)
 - JKang.EventSourcing.Persistence.DynamoDB [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.DynamoDB.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.DynamoDB)
 - JKang.EventSourcing.Persistence.CosmosDB [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.CosmosDB.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.CosmosDB)
 - JKang.EventSourcing.Persistence.S3 [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.S3.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.S3)
 - JKang.EventSourcing.Persistence.Caching [![NuGet version](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.Caching.svg)](https://badge.fury.io/nu/JKang.EventSourcing.Persistence.Caching)

## Quick Start:

Let's implement a simple gift card management system with the following use cases:
 * Create gift cards with initial credit
 * Debit the gift card specifying amount while overpaying is not allowed

I'm adopting *DDD (Domain Driven Design)* approach and implement the *GiftCard* entity as an **Rich Domain Aggregate** which encapsulates/protects its internal data/state, and contains itself business logics ensuring data integrity.

### Step 1 - Create aggregate events

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

Notes: 
 - It's recommended to implement aggregate event in an immutable way.
 - Inheriting from `AggregateEvent<TKey>` or `AggregateCreatedEvent<TKey>` is not mandatory, but an aggreagte event must at least implement  `IAggregateEvent<TKey>` interface.
 - In order to use built-in event stores, please make sure event can be properly serialized using [Json.NET](https://www.newtonsoft.com/json).

### Step 2 - Create domain aggregate

```csharp
public class GiftCard : Aggregate<Guid>
{
    /// <summary>
    /// Constructor for creating an new gift card from scratch
    /// </summary>
    public GiftCard(decimal initialCredit)
        : base(new GiftCardCreated(Guid.NewGuid(), DateTime.UtcNow, initialCredit))
    { }

    /// <summary>
    /// Constructor for rehydrating gift card from historical events
    /// </summary>
    public GiftCard(Guid id, IEnumerable<IAggregateEvent<Guid>> savedEvents)
        : base(id, savedEvents)
    { }

    /// <summary>
    /// Constructor for rehydrating gift card from a snapshot + historical events after the snapshot
    /// </summary>
    public GiftCard(Guid id, IAggregateSnapshot<Guid> snapshot, IEnumerable<IAggregateEvent<Guid>> savedEvents)
        : base(id, snapshot, savedEvents)
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

Notes:
 - Please ensure that state of domain aggregate can only be changed by applying aggregate events. 
 - Inheriting from `Aggregate<TKey>` is not mandatory, but the minimum requirements for implementing a domain aggregate are:
   - Implement `IAggregate<TKey>` interface
   - Have a public constructor with signature `MyAggregate(TKey id, IEnumerable<IAggregateEvent<TKey>> savedEvents)`
   - Have a public constructor with signature `MyAggregate(TKey id, IAggregateSnapshot<TKey> snapshot, IEnumerable<IAggregateEvent<TKey>> savedEvents)`


### Step 3 - Implement repository

By definition of Event Sourcing, persisting an aggregate insists on persisting all historical events.

```csharp
public interface IGiftCardRepository
{
    Task SaveGiftCardAsync(GiftCard giftCard);
    Task<GiftCard> FindGiftCardAsync(Guid id);
}
```
    
```csharp
public class GiftCardRepository : AggregateRepository<GiftCard, Guid>, 
    IGiftCardRepository
{
    public GiftCardRepository(IEventStore<GiftCard, Guid> eventStore)
        : base(eventStore)
    { }

    public Task SaveGiftCardAsync(GiftCard giftCard) =>
        SaveAggregateAsync(giftCard);

    public Task<GiftCard> FindGiftCardAsync(Guid id) =>
        FindAggregateAsync(id);
}
```

### Step 4 - Register your repository interface and configure event store in dependency injection framework

```csharp
services
    .AddScoped<IGiftCardRepository, GiftCardRepository>();

services
    .AddEventSourcing(builder =>
    {
        builder.UseTextFileEventStore<GiftCard, Guid>(x =>
            x.Folder = "C:/Temp/GiftcardEvents");
    });
```

Notes:
 - You can choose other persistence store provided such as [CosmosDB](doc/CosmosDBSetup.md) or [DynamoDB](doc/DynamoDBSetup) etc.

### Step 5 - implmement use cases

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

## FAQs

### How to programmatically initialize event store?

See [this page](doc/StoreInitialization.md).

### How to use snapshots to optimize performance?

See [this page](doc/Snapshots.md).

### How to improve performance using caching?

Consider install the nuget package `JKang.EventSourcing.Persistence.Caching` and inherit the `CachedAggregateRepository` class.
It leverages `Microsoft.Extensions.Caching.Distributed.IDistributedCache` to cache aggregate every time after loaded from or saved into repository.

Consider configuring a short sliding expiration (e.g., 5 sec) to reduce the chance of having cache out of date.

---
__Please feel free to download, fork and/or provide any feedback!__
