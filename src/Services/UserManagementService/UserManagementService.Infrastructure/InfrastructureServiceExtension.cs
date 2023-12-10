using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Infrastructure.ApiGateway;
using UserManagementService.Infrastructure.Notifications;
using UserManagementService.Infrastructure.Notifications.Strategies;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementService.Infrastructure;

public static class InfrastructureServiceExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection collection)
    {
        collection.AddScoped<IEventBus, PubSubEventBus>();
        collection.AddScoped<ISendNotificationStrategyFactory, FcmSendNotificationSendNotificationStrategyFactory>();
        collection.AddScoped<INotifier, FcmNotifier>();
        collection.AddScoped<IConnectionStringManager, ConnectionStringManager>();
        
        return collection;
    }
}