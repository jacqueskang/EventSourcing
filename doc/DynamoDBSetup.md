# Setup DynamoDB event store

1. Install NuGet package [JKang.EventSourcing.Persistence.DynamoDB](https://www.nuget.org/packages/JKang.EventSourcing.Persistence.DynamoDB/)

    ```powershell
    PM> Install-Package JKang.EventSourcing.Persistence.DynamoDB
    ```

1. Register event sourcing services in ConfigureServices()

    ```csharp
    services
        .AddDefaultAWSOptions(Configuration.GetAWSOptions())
        .AddAWSService<IAmazonDynamoDB>();
    
    services
        .AddEventSourcing(builder =>
        {
            builder.UseDynamoDBEventStore<GiftCard, Guid>(x =>
                x.TableName = "GiftcardEvents");
        });
    ```

*Hint: Consider using [DynamoDB local](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.html) in local development environment:*

1. Install [Docker](https://docs.docker.com/install/)

1. Run DynamoDB local
   ```shell
   $ docker run -p 8000:8000 amazon/dynamodb-local
   ```

1. Update ConfigureServices() with:
   ```csharp
    #if DEBUG
        services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000"
        }));
    #else
        services.AddAWSService<IAmazonDynamoDB>();
    #endif
   ```
