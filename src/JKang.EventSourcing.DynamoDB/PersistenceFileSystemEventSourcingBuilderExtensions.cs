using Amazon.DynamoDBv2;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.DynamoDB;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceDynamoDBEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDynamoDBEventStore<TAggregate, TAggregateKey>(
            this IEventSourcingBuilder builder,
            IConfiguration configuration)
            where TAggregate : IAggregate<TAggregateKey>
        {
            builder.Services
                .Configure<DynamoDBEventStoreOptions>(typeof(TAggregate).FullName, configuration)
                .AddAWSService<IAmazonDynamoDB>()
                .AddScoped<IEventStore<TAggregate, TAggregateKey>, DynamoDBEventStore<TAggregate, TAggregateKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TAggregateKey>, DynamoDBEventStoreInitializer<TAggregate, TAggregateKey>>()
                ;
            return builder;
        }
    }
}
