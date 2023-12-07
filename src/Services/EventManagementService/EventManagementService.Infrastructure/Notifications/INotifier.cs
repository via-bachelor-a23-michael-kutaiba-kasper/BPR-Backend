using EventManagementService.Infrastructure.Notifications.Models;

namespace EventManagementService.Infrastructure.Notifications;

public interface INotifier
{
    public Task SendNotificationAsync(Notification notification);
}