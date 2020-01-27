# Setup CosmosDB event store

## Change codes in Startup.cs to use CosmosDB

1. Install NuGet package [JKang.EventSourcing.Persistence.CosmosDB](https://www.nuget.org/packages/JKang.EventSourcing.Persistence.CosmosDB/)

    ```powershell
    PM> Install-Package JKang.EventSourcing.Persistence.CosmosDB
    ```

1. Register CosmosClient in ConfigureServices()

    ```csharp
	services.AddSingleton(_ =>
		new CosmosClientBuilder(Configuration.GetConnectionString("CosmosDB"))
			.WithConnectionModeDirect()
			.WithCustomSerializer(new EventSourcingCosmosSerializer())
			.Build());
    ```

1. (Optional) If you want automatically create the table during application startup:

    ```csharp
    public void Configure(
        IApplicationBuilder app,
        IEventStoreInitializer<GiftCard, Guid> giftCardStoreInitializer)
    {
        giftCardStoreInitializer.EnsureCreatedAsync().Wait();

        // other configurations...
    }
    ```
