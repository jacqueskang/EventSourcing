using JKang.EventSourcing.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Samples.Domain;
using Samples.Persistence;
using Samples.WebApp.Data;
using System;

namespace Samples.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services
                .AddScoped<IGiftCardRepository, GiftCardRepository>()
                ;

            services
                .AddDbContext<SampleDbContext>(x =>
                {
                    x.UseInMemoryDatabase("eventstore");
                });

            services
                .AddEventSourcing(builder =>
                {
                    builder
                        .UseEfCoreEventStore<SampleDbContext, GiftCard, Guid>();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            var eventStoreInitializer = app.ApplicationServices.GetRequiredService<IEventStoreInitializer<GiftCard, Guid>>();
            eventStoreInitializer.EnsureCreatedAsync().Wait();

            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
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
