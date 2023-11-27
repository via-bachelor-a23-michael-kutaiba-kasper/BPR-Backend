using EventManagementService.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Infrastructure;

public static class InfrastructureServiceExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection collection)
    {
        collection.AddScoped<IEventBus, PubSubEventBus>();
        
        return collection;
    }
}