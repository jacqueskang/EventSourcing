# Setup file system event store

1. Install NuGet package [JKang.EventSourcing.Persistence.FileSystem](https://www.nuget.org/packages/JKang.EventSourcing.Persistence.FileSystem/)

    ```powershell
    PM> Install-Package JKang.EventSourcing.Persistence.FileSystem
    ```

1. Register event sourcing services in ConfigureServices()

    ```csharp
	services
        .AddEventSourcing(builder =>
        {
            builder.UseTextFileEventStore<GiftCard, Guid>(x =>
                x.Folder = "C:/Temp/GiftcardEvents");
        });
    ```
