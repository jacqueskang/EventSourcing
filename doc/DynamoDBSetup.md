# Setup DynamoDB event store

1. Install NuGet package [JKang.EventSourcing.Persistence.DynamoDB](https://www.nuget.org/packages/JKang.EventSourcing.Persistence.DynamoDB/)

    ```powershell
    PM> Install-Package JKang.EventSourcing.Persistence.DynamoDB
    ```

1. Register necessary services in ConfigureServices()

    ```csharp
    services
        .AddDefaultAWSOptions(Configuration.GetAWSOptions())
        .AddAWSService<IAmazonDynamoDB>()
        .AddEventSourcing(builder =>
        {
            builder.UseDynamoDBEventStore<GiftCard, Guid>(x => x.TableName = "GiftcardEvents");
        });
    ```

1. (Optional) If you want automatically create the DynamoDB table during application startup:

    ```csharp
    public void Configure(
        IApplicationBuilder app,
        IEventStoreInitializer<GiftCard, Guid> giftCardStoreInitializer)
    {
        giftCardStoreInitializer.EnsureCreatedAsync().Wait();

        //...
    }
    ```

## Personal preferences for configuring DynamoDB connection in different environments

### Development environment with remote DynamoDB in AWS

    Follow https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-netcore.html to configure AWS profile in appsettings.json

    ```json
    {
    "AWS": {
        "Profile": "default",
        "Region": "eu-west-1"
    }
    }
    ```

### Development environment with locally installed DynamoDB emulator

1. Download and start DynamoDB Local following https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.html

1. Assuming local DynamoDB is listening port 8000, update ConfigureServices() with:

    ```csharp
    if (HostingEnvironment.IsDevelopment())
    {
        services.AddSingleton<IAmazonDynamoDB>(sp => new AmazonDynamoDBClient(new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000"
        }));
    }
    else
    {
        services
            .AddDefaultAWSOptions(Configuration.GetAWSOptions())
            .AddAWSService<IAmazonDynamoDB>();
    }

    services
        .AddEventSourcing(builder =>
        {
            builder.UseDynamoDBEventStore<GiftCard, Guid>(x => x.TableName = "GiftcardEvents");
        });
    ```

### Production environment

    Configure an IAM role with approperate permissions, then attach the role to the execution resource (EC2 instance, ECS task or Lambda function...). AWS SDK will automatically authenticate against DynamoDB using STS.
