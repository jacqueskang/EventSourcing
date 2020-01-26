# Setup DynamoDB event store

## Change codes in Startup.cs to use DynamoDB

1. Configure DI in ConfigureServices()
```csharp
    services
        .AddDefaultAWSOptions(Configuration.GetAWSOptions())
        .AddAWSService<IAmazonDynamoDB>();
        
    services
        .AddEventSourcing(builder =>
        {
            builder
                .UseDynamoDBEventStore<GiftCard, Guid>(x => x.TableName = "GiftcardEvents");
        });
```

2. (Optional) If you want automatically create the table during application startup, inject `IEventStoreInitializer<, >` in Configure() and call `EnsureCreatedAsync()`:
```csharp
        public void Configure(
            IApplicationBuilder app,
            IEventStoreInitializer<GiftCard, Guid> giftCardStoreInitializer)
        {
            giftCardStoreInitializer.EnsureCreatedAsync().Wait();
            
            // other configurations...
        }
```

## Configure DynamoDB in appsettings.json

### Work with remote DynamoDB in AWS (for development or testing)

1. Follow https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-netcore.html to configure AWS profile in appsettings.json

2. Configure event store in appsettings.json with:
```json
{
  "AWS": {
    "Profile": "default",
    "Region": "eu-west-1"
  }
}
```

### Work with local installed DynamoDB (for development or testing)

1. Download and start DynamoDB Local following https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.html
2. Assuming local DynamoDB is listening port 8000, update ConfigureServices() with:

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
```

### Work with DynamoDB in AWS (for production)

1. Configure an IAM role with approperate permissions, then attach the role to the execution resource (EC2 instance, ECS task or Lambda function...). AWS SDK will automatically authenticate against DynamoDB using STS.
