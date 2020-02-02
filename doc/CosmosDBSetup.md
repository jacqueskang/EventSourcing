# Setup CosmosDB event store

1. Install NuGet package [JKang.EventSourcing.Persistence.CosmosDB](https://www.nuget.org/packages/JKang.EventSourcing.Persistence.CosmosDB/)

    ```powershell
    PM> Install-Package JKang.EventSourcing.Persistence.CosmosDB
    ```

1. Register event sourcing services in ConfigureServices()

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
