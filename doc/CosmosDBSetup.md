# Setup CosmosDB event store

1. Install NuGet package [JKang.EventSourcing.Persistence.CosmosDB](https://www.nuget.org/packages/JKang.EventSourcing.Persistence.CosmosDB/)

    ```powershell
    PM> Install-Package JKang.EventSourcing.Persistence.CosmosDB
    ```

1. Register necessary services in ConfigureServices()

    ```csharp
	services
        .AddSingleton(_ =>
            new CosmosClientBuilder(Configuration.GetConnectionString("CosmosDB"))
                .WithConnectionModeDirect()
                .WithCustomSerializer(new EventSourcingCosmosSerializer())
                .Build())
        .AddEventSourcing(builder =>
        {
            builder.UseCosmosDBEventStore<GiftCard, Guid>(x =>
            {
                x.DatabaseId = "EventSourcingTestingWebApp";
                x.ContainerId = "GiftcardEvents";
            });
        });
    ```

1. (Optional) If you want automatically create the CosmosDB database and collection during application startup:

    ```csharp
    public void Configure(
        IApplicationBuilder app,
        IEventStoreInitializer<GiftCard, Guid> giftCardStoreInitializer)
    {
        giftCardStoreInitializer.EnsureCreatedAsync().Wait();

        //...
    }
    ```
