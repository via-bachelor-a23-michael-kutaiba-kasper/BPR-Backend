using EventManagementService.Infrastructure.EventBus;
using EventManagementService.Infrastructure.Notifications;
using EventManagementService.Infrastructure.Notifications.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Infrastructure;

public static class InfrastructureServiceExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection collection)
    {
        collection.AddScoped<IEventBus, PubSubEventBus>();
        collection.AddScoped<ISendNotificationStrategyFactory, FcmSendNotificationSendNotificationStrategyFactory>();
        collection.AddScoped<INotifier, FcmNotifier>();
        
        return collection;
    }
}