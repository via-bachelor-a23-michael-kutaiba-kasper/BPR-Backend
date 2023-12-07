using UserManagementService.Infrastructure.Notifications.Models;

namespace UserManagementService.Infrastructure.Notifications.Strategies;

public interface ISendNotificationStrategy
{
    public Task Send(Notification notification);
}