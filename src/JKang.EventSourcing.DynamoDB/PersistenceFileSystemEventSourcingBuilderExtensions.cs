using Amazon.DynamoDBv2;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.DynamoDB;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceDynamoDBEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDynamoDBEventStore<TAggregate, TAggregateKey>(
            this IEventSourcingBuilder builder,
            IConfiguration configuration,
            Action<DynamoDBEventStoreOptions> setupAction)
            where TAggregate : IAggregate<TAggregateKey>
        {
            builder.Services
                .Configure(setupAction)
                .AddDefaultAWSOptions(configuration.GetAWSOptions())
                .AddAWSService<IAmazonDynamoDB>()
                .AddScoped<IEventStore<TAggregate, TAggregateKey>, DynamoDBEventStore<TAggregate, TAggregateKey>>()
                ;
            return builder;
        }
    }
}
