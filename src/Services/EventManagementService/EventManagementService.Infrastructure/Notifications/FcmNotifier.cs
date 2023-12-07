using EventManagementService.Infrastructure.Notifications.Models;
using EventManagementService.Infrastructure.Notifications.Strategies;

namespace EventManagementService.Infrastructure.Notifications;

public class FcmNotifier: INotifier
{
    private readonly ISendNotificationStrategyFactory _factory;
    
    public FcmNotifier(ISendNotificationStrategyFactory factory)
    {
        _factory = factory;
    }
    
    public async Task SendNotificationAsync(Notification notification)
    {
        var strategy = _factory.Create(notification);
        await strategy.Send(notification);
    }
}