using Microsoft.Extensions.DependencyInjection;

namespace RecommendationService.Infrastructure;

public static class ServiceExtension
{
    public IServiceCollection AddInfrastructureServices(this IServiceCollection collection)
    {
        collection.AddScoped<IEventBus, PubSubEventBus>();
        collection.AddScoped<IConnectionStringManager, ConnectionStringManager>();
        
        return collection;
    }
}