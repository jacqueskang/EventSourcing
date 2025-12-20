# Copilot instructions — EventSourcing (JKang.EventSourcing)

Purpose: help an AI coding agent quickly be productive in this repo — what matters, where to look, and how to run/verify work.

## Quick summary
- This is a .NET Core event-sourcing framework (see `README.md`).
- Core projects: `JKang.EventSourcing.*` (abstractions, options, snapshotting, persistence implementations).
- Persistence providers are separate projects under `src/` (e.g., `Persistence.FileSystem`, `Persistence.DynamoDB`, `Persistence.CosmosDB`, `Persistence.EfCore`, `Persistence.S3`).
- `JKang.EventSourcing.TestingWebApp` is an interactive sample app to exercise different persistence backends.

## Useful commands (local dev)
- Restore/build all: `dotnet restore` then `dotnet build src\EventSourcing.sln`
- Run tests: `dotnet test src` (or `dotnet test` inside a specific `*.Tests` project)
- Run the sample web app: `dotnet run --project src\JKang.EventSourcing.TestingWebApp\JKang.EventSourcing.TestingWebApp.csproj`
  - Toggle storage backend inside `Startup.cs` (see `PersistenceMode` and `ConfigureServicesFor*` methods) or set `appsettings.json` values for AWS/Cosmos.
- CI: Azure Pipelines YAML lives under `build/` (e.g., `azure-pipeline.yml`, `pack-publish.yml`).

## Architecture & important files to read first
- High-level: core abstractions and patterns are in `JKang.EventSourcing.Abstractions` and `JKang.EventSourcing`.
- Examples & canonical patterns: `src\JKang.EventSourcing.TestingFixtures\` (e.g., `GiftCard.cs`, `GiftCardSnapshot.cs`, `GiftCardRepository.cs`) and `samples/`.
- Persistence pattern: each store provides
  - Event store (`IEventStore<TAgg,TKey>` implementation) — e.g., `TextFileEventStore`, `DynamoDBEventStore`, `EfCoreEventStore`.
  - Snapshot store (`ISnapshotStore<TAgg,TKey>`) when supported.
  - Builder extension to wire into DI: `*PersistenceEventSourcingBuilderExtensions.cs` (e.g., `S3SnapshotPersistenceEventSourcingBuilderExtensions.cs`).
  - `Defaults.cs` with Json.NET settings used consistently across providers.
- Initializers: `IEventStoreInitializer<>` / `ISnapshotStoreInitializer<>` — used by `TestingWebApp` to create tables/containers before use.
- Serializers: Cosmos uses `EventSourcingCosmosSerializer` and persistence uses `Defaults.JsonSerializerSettings`. Ensure any new event types are JSON-serializable with these settings.

## Project-specific conventions & patterns
- Aggregate pattern: aggregates implement `IAggregate<TKey>` (often via `Aggregate<TKey>`) and mutate state only by applying `IAggregateEvent<TKey>` in `ApplyEvent`.
  - Required constructors: `(TKey id, IEnumerable<IAggregateEvent<TKey>> savedEvents)` and `(TKey id, IAggregateSnapshot<TKey> snapshot, IEnumerable<IAggregateEvent<TKey>> savedEvents)` for rehydration.
  - Snapshot support via `TakeSnapshot()` and `IAggregateSnapshot`.
- Events should be immutable and JSON-serializable. Many stores call `JsonConvert.SerializeObject(@event, Defaults.JsonSerializerSettings)`.
- Naming: options & configuration types end with `Options` (e.g., `S3SnapshotStoreOptions`), and extension classes follow the `*PersistenceEventSourcingBuilderExtensions` pattern.
- Async-first: APIs and store methods use `async`/`Task` and take optional `CancellationToken`.

## How to add a new persistence provider (pattern to follow)
1. Create `JKang.EventSourcing.Persistence.YourProvider` project under `src/`.
2. Add `Defaults.cs` for serializer settings (reuse pattern from existing `Defaults.cs`).
3. Implement `IEventStore<TAggregate,TKey>` and (optionally) `ISnapshotStore<TAggregate,TKey>`.
4. Provide builder extension `YourProviderPersistenceEventSourcingBuilderExtensions.cs` that:
   - Adds configuration via `.ConfigureAggregate<TAggregate,TKey,YourOptions>(...)` and registers the services in DI.
5. Add initializer implementing `IEventStoreInitializer<>` / `ISnapshotStoreInitializer<>` if the backend needs resource creation (tables, containers, buckets).
6. Create tests mirroring existing provider tests (see `*.Tests` projects) and a sample configuration in `TestingWebApp`.

## Testing & local integration tips
- Unit tests use xUnit (see `*.Tests` projects under `src/`). Use `dotnet test`.
- DynamoDB local: `TestingWebApp` uses `ServiceURL = "http://localhost:8800"` under DEBUG — useful with a local DynamoDB container (e.g., LocalStack or dynamodb-local).
- Cosmos: `ConfigureServicesForCosmosDB` expects a connection string under `ConnectionStrings:CosmosDB` (see `Startup.cs`).
- S3 and DynamoDB integration tests assume AWS credentials or local endpoints configured in environment or `appsettings.json`.

## What to check when changing behavior
- Verify serialization round-trip: events and snapshots must deserialize correctly with `Defaults.JsonSerializerSettings` (or `EventSourcingCosmosSerializer` for Cosmos).
- Make sure aggregate invariants are enforced in `ApplyEvent` or when emitting events (tests often assert that invalid ops throw `InvalidOperationException`).
- Update `TestingWebApp` to allow manual verification of a new storage configuration.

## Where to look for examples / canonical code
- Aggregate + events: `src\JKang.EventSourcing.TestingFixtures\GiftCard.cs`, `GiftCardCreated.cs`, `GiftCardDebited.cs`.
- Text file provider: `src\JKang.EventSourcing.Persistence.FileSystem\` (simple, readable reference implementation).
- EF Core provider + snapshots: `src\JKang.EventSourcing.Persistence.EfCore\`.
- DynamoDB provider: `src\JKang.EventSourcing.Persistence.DynamoDB\` (good example for non-relational storage patterns).
- Testing web app: `src\JKang.EventSourcing.TestingWebApp\Startup.cs` (how to wire different providers at runtime).

---
If you want, I can merge this into any existing `.github/copilot-instructions.md` (if present) or iterate with more details (e.g., example PR checklist, coding style rules, or common bug patterns). Please tell me which additions you'd like.  

*Generated: concise, practical, and based on discoverable patterns in this repo.*
