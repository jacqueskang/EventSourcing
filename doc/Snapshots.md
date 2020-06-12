# How to use snapshots to optimize performance?

Performance can become an issue when an aggregate has a large amout of historical events, because all these events must be loaded and compiled to calculate the current state of the aggregate. Snapshot can help resolving this issue.

The idea is simple: You take a snapshot of the aggregate when necessary and save it in another store (snapshot store). Then to rehydrate the aggregate again you can save IO and computation by only retrieving the snapshot and events that occurred after that snapshot.

To enable this feature you need to implement the following:

1. Add the following constructor to the domain aggregate:
   ```csharp
    public GiftCard(
        Guid id,
        IAggregateSnapshot<Guid> snapshot,
        IEnumerable<IAggregateEvent<Guid>> savedEvents)
        : base(id, snapshot, savedEvents)
        { }
   ```

1. Create a class that represents the snapshot
    ```csharp
    public class GiftCardSnapshot : AggregateSnapshot<Guid>
    {
        public GiftCardSnapshot(Guid aggregateId, int aggregateVersion,
            decimal balance)
            : base(aggregateId, aggregateVersion)
        {
            Balance = balance;
        }

        public decimal Balance { get; }
    }
    ```

1. Override the `CreateSnapshot()` method in aggregate
   ```csharp
    protected override IAggregateSnapshot<Guid> CreateSnapshot()
    {
        return new GiftCardSnapshot(Id, Version, Balance);
    }
   ```

1. Override the `ApplySnapshot()` method
   ```csharp
    protected override void ApplySnapshot(IAggregateSnapshot<Guid> snapshot)
    {
        GiftCardSnapshot giftCardSnapshot = snapshot as GiftCardSnapshot
            ?? throw new InvalidOperationException();

        Balance = giftCardSnapshot.Balance;
    }
   ```

   Note: 
   - It is your resposibility to ensure that your snapshot class contains all data needed to restore that aggregate to the corresponding version.
   - To use built-in stores, make sure that your snapshot class can be serialized/deserialized by Json.NET

1. Register snapshot store in `IEventSourcingBuilder`, in a similar way as registering the event stores, e.g.,

    ```csharp
    services.AddEventSourcing(builder =>
        builder
            .UseTextFileEventStore<GiftCard, Guid>(x => x.Folder = "C:/Temp/GiftcardEvents")
            .UseTextFileSnapshotStore<GiftCard, Guid>(x => x.Folder = "C:/Temp/GiftcardEvents"));
    ```

   Note: You can store snapshot in the same or in a different storage with events. For example: store events in DynamoDB and store snapshots in text files:

    ```csharp
    services.AddEventSourcing(builder =>
        builder
            .UseDynamoDBEventStore<GiftCard, Guid>(x => x.TableName = "GiftcardEvents")
            .UseTextFileSnapshotStore<GiftCard, Guid>(x => x.Folder = "C:/Temp/GiftcardEvents"));
    ```

1. Optionally you can programatically initialize snapshot store in a similar way with event store ([see here](StoreInitialization.md)):
    ```csharp
    public void Configure(IApplicationBuilder app,
        IWebHostEnvironment env,
        IEventStoreInitializer<GiftCard, Guid> eventStoreInitializer,
        ISnapshotStoreInitializer<GiftCard, Guid> snapshotStoreInitializer)
    {
        eventStoreInitializer.EnsureCreatedAsync().Wait();
        snapshotStoreInitializer.EnsureCreatedAsync().Wait();

        // configure http request pipeline
    }
    ```

1. It's totally up to you to decide when to take an snapshot by calling `aggregate.TakeSnapshot()`. The snapshot will be saved when invoked `AggregateRepository<TAggregate, TKey>.SaveAggregateAsync()`

1. By default when calling `AggregateRepository<TAggregate, TKey>.FindAggregateAsync()` it automatically retrieve the last snapshot. You can force not loading any snapshot by calling:
    ```csharp
    FindAggregateAsync(aggregateId, ignoreSnapshot: true)
    ```