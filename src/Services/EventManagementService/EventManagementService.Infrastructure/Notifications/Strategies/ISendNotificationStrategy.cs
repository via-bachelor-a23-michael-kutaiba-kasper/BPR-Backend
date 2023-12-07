using EventManagementService.Infrastructure.Notifications.Models;

namespace EventManagementService.Infrastructure.Notifications.Strategies;

public interface ISendNotificationStrategy
{
    public Task Send(Notification notification);
}