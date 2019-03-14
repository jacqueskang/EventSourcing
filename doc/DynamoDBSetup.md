# Setup DynamoDB event store

## Change codes in Startup.cs to use DynamoDB

1. Configure DI in ConfigureServices()
```json
    services
        .AddDefaultAWSOptions(Configuration.GetAWSOptions())
        .AddEventSourcing(builder =>
        {
            builder
                .UseJsonEventSerializer()
                .UseDynamoDBEventStore<GiftCard, Guid>(Configuration.GetSection("GiftCardEventStore"))
                ;
        });
```

2. (Optional) If you want automatically create the table during application startup, inject `IEventStoreInitializer<, >` in Configure() and call `EnsureCreatedAsync()`:
```csharp
        public void Configure(IApplicationBuilder app,
            IEventStoreInitializer<GiftCard, Guid> eventStoreInitializer)
        {
            eventStoreInitializer.EnsureCreatedAsync().Wait();
            
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
  },
  "GiftCardEventStore": {
    "TableName": "GiftCardEvents"
  }
}
```

### Work with local installed DynamoDB (for development or testing)

1. Download and start DynamoDB Local following https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.html
2. Configure event store in appsettings.json with:

```json
{
  "AWS": {
    "Profile": "default",
    "Region": "eu-west-1"
  },
  "GiftCardEventStore": {
    "TableName": "GiftCardEvents",
    "UseLocalDB": true,
    "ServiceURL": "http://localhost:8888"
  }
}
```

### Work with DynamoDB in AWS (for production)

1. Configure an IAM role with approperate permissions, then attach the role to the execution resource (EC2 instance, ECS task or Lambda function...)
2. Configure event store in appsettings.json with:
```json
{
  "GiftCardEventStore": {
    "TableName": "GiftCardEvents"
  }
}
```