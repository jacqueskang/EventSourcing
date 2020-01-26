using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using AutoFixture.Xunit2;
using JKang.EventSourcing.TestingFixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace JKang.EventSourcing.Persistence.DynamoDB.Tests
{
    public class ServiceBuilderTests
    {
        [Theory, AutoData]
        public void UseDynamoDBEventStore_HappyPath(string tableName, RegionEndpoint region)
        {
            ServiceProvider sp = new ServiceCollection()
                .AddDefaultAWSOptions(new AWSOptions
                {
                    Region = region
                })
                .AddAWSService<IAmazonDynamoDB>()
                .AddEventSourcing(builder => builder
                    .UseDynamoDBEventStore<GiftCard, Guid>(x => x.TableName = tableName))
                .BuildServiceProvider();

            using IServiceScope scope = sp.CreateScope();

            IAmazonDynamoDB actual = scope.ServiceProvider.GetService<IAmazonDynamoDB>();
            Assert.NotNull(actual);
            Assert.Equal(region, actual.Config.RegionEndpoint);

            IOptionsMonitor<DynamoDBEventStoreOptions> monitor = scope.ServiceProvider.GetService<IOptionsMonitor<DynamoDBEventStoreOptions>>();
            Assert.NotNull(monitor);
            DynamoDBEventStoreOptions value = monitor.Get(typeof(GiftCard).FullName);
            Assert.Equal(tableName, value.TableName);
        }
    }
}
