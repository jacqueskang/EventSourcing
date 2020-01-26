using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.TestingFixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Amazon.DynamoDBv2;
using JKang.EventSourcing.Persistence.CosmosDB;

namespace JKang.EventSourcing.TestingWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services
                .AddScoped<IGiftCardRepository, GiftCardRepository>();

            // change the following value to switch persistence mode
            PersistenceMode persistenceMode = PersistenceMode.CosmosDB;

            switch (persistenceMode)
            {
                case PersistenceMode.DynamoDB:
                    if (HostingEnvironment.IsDevelopment())
                    {
                        services.AddSingleton<IAmazonDynamoDB>(sp => new AmazonDynamoDBClient(new AmazonDynamoDBConfig
                        {
                            ServiceURL = "http://localhost:8800"
                        }));
                    }
                    else
                    {
                        services.AddAWSService<IAmazonDynamoDB>();
                    }
                    break;
                case PersistenceMode.CosmosDB:
                    services.AddSingleton(_ =>
                        new CosmosClientBuilder(Configuration.GetConnectionString("CosmosDB"))
                            .WithConnectionModeDirect()
                            .WithCustomSerializer(new EventSourcingCosmosSerializer())
                            .Build());
                    break;
                default:
                    break;
            }

            services.AddEventSourcing(builder =>
            {
                switch (persistenceMode)
                {
                    case PersistenceMode.DynamoDB:
                        builder.UseDynamoDBEventStore<GiftCard, Guid>(
                            x => x.TableName = "GiftcardEvents");
                        break;
                    case PersistenceMode.CosmosDB:
                        builder.UseCosmosDBEventStore<GiftCard, Guid>(x =>
                        {
                            x.DatabaseId = "EventSourcingTestingWebApp";
                            x.ContainerId = "GiftcardEvents";
                        });
                        break;
                    default:
                        break;
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IEventStoreInitializer<GiftCard, Guid> giftCardStoreInitializer)
        {
            giftCardStoreInitializer.EnsureCreatedAsync().Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
