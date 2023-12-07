using EventManagementService.Infrastructure.Notifications.Models;

namespace EventManagementService.Infrastructure.Notifications.Strategies;

public interface ISendNotificationStrategyFactory
{
    public ISendNotificationStrategy Create(Notification notification);
}
