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
                .Configure<DynamoDBEventStoreOptions>(configuration)
                .AddAWSService<IAmazonDynamoDB>()
                .AddScoped<IEventStore<TAggregate, TAggregateKey>, DynamoDBEventStore<TAggregate, TAggregateKey>>()
                ;
            return builder;
        }
    }
}
