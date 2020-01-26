using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.TestingFixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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
                .AddScoped<IGiftCardRepository, GiftCardRepository>()
                .AddEventSourcing(builder =>
                {
                    // change the following value to switch persistence mode
                    PersistenceMode persistenceMode = PersistenceMode.DynamoDb;
                    switch (persistenceMode)
                    {
                        case PersistenceMode.DynamoDb:
                            if (HostingEnvironment.IsDevelopment())
                            {
                                builder.UseLocalDynamoDBEventStore<GiftCard, Guid>(
                                    x => x.TableName = "GiftcardEvents",
                                    new Uri("http://localhost:8800"));
                            }
                            else
                            {
                                builder.UseDynamoDBEventStore<GiftCard, Guid>(
                                    x => x.TableName = "GiftcardEvents");
                            }
                            break;
                        default:
                            break;
                    }
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IEventStoreInitializer<GiftCard, Guid> eventStoreInitializer)
        {
            eventStoreInitializer.EnsureCreatedAsync().Wait();

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
