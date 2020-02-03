using JKang.EventSourcing.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc();

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
            IEventStoreInitializer<GiftCard, Guid> eventStoreInitializer)
        {
            eventStoreInitializer.EnsureCreatedAsync().Wait();

            app.UseExceptionHandler("/Error");
            app.UseHsts();

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
