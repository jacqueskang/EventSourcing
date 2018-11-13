using Samples.Domain;
using Samples.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            return services
                .AddScoped<IGiftCardRepository, GiftCardRepository>()
                ;
        }
    }
}
