using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Persistence.DynamoDB;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceDynamoDBEventSourcingBuilderExtensions
    {
        public static IEventSourcingBuilder UseDynamoDBEventStore<TAggregate, TAggregateKey>(
            this IEventSourcingBuilder builder,
            Action<DynamoDBEventStoreOptions> setupAction)
            where TAggregate : IAggregate<TAggregateKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .AddCommonServices<TAggregate, TAggregateKey>(setupAction)
                .TryAddAWSService<IAmazonDynamoDB>()
                ;

            return builder;
        }

        public static IEventSourcingBuilder UseDynamoDBEventStore<TAggregate, TAggregateKey>(
            this IEventSourcingBuilder builder,
            Action<DynamoDBEventStoreOptions> setupAction,
            AWSOptions awsOptions)
            where TAggregate : IAggregate<TAggregateKey>
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .AddCommonServices<TAggregate, TAggregateKey>(setupAction)
                .TryAddAWSService<IAmazonDynamoDB>(awsOptions)
                ;

            return builder;
        }

        public static IEventSourcingBuilder UseLocalDynamoDBEventStore<TAggregate, TAggregateKey>(
            this IEventSourcingBuilder builder,
            Action<DynamoDBEventStoreOptions> setupAction,
            Uri dynamoDbServiceUrl)
            where TAggregate : IAggregate<TAggregateKey>
        {
            if (builder is null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            builder.Services
                .AddCommonServices<TAggregate, TAggregateKey>(setupAction)
                .AddSingleton<IAmazonDynamoDB>(sp => new AmazonDynamoDBClient(new AmazonDynamoDBConfig
                {
                    ServiceURL = dynamoDbServiceUrl.ToString()
                }))
                ;

            return builder;
        }

        private static IServiceCollection AddCommonServices<TAggregate, TAggregateKey>(
            this IServiceCollection services,
            Action<DynamoDBEventStoreOptions> setupAction)
            where TAggregate : IAggregate<TAggregateKey>
        {
            return services
                .Configure(typeof(TAggregate).FullName, setupAction)
                .AddScoped<IEventStore<TAggregate, TAggregateKey>, DynamoDBEventStore<TAggregate, TAggregateKey>>()
                .AddScoped<IEventStoreInitializer<TAggregate, TAggregateKey>, DynamoDBEventStoreInitializer<TAggregate, TAggregateKey>>()
                ;
        }
    }
}
