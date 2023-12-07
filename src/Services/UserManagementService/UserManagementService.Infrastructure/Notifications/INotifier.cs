using UserManagementService.Infrastructure.Notifications.Models;

namespace UserManagementService.Infrastructure.Notifications;

public interface INotifier
{
    public Task SendNotificationAsync(Notification notification);
}