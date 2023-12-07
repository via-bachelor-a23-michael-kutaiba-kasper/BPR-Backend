using UserManagementService.Infrastructure.Notifications.Models;
using UserManagementService.Infrastructure.Notifications.Strategies;

namespace UserManagementService.Infrastructure.Notifications;

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