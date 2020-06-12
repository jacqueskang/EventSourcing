# How to programmatically initialize event store?

Sometimes it's handy to programmatically initialize event store. Do do this you can resolve `IEventStoreInitializer<TAggregate, TKey>` from DI service provider and call `EnsureCreatedAsync()`.

For example to initialize event store when ASP.NET Core application is started:

```csharp
public void Configure(IApplicationBuilder app,
    IWebHostEnvironment env,
    IEventStoreInitializer<GiftCard, Guid> eventStoreInitializer)
{
    eventStoreInitializer.EnsureCreatedAsync().Wait();

    // configure http request pipelines
}
```