using UserManagementService.Infrastructure.Notifications.Models;

namespace UserManagementService.Infrastructure.Notifications.Strategies;

public interface ISendNotificationStrategyFactory
{
    public ISendNotificationStrategy Create(Notification notification);
}