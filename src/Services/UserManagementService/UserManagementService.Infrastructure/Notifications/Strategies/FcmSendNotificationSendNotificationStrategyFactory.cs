using UserManagementService.Infrastructure.Notifications.Models;

namespace UserManagementService.Infrastructure.Notifications.Strategies;

public class FcmSendNotificationSendNotificationStrategyFactory: ISendNotificationStrategyFactory
{
    public ISendNotificationStrategy Create(Notification message)
    {
        if (message is UserNotification)
        {
            return new SendUserMessageStrategy();
        }

        throw new Exception("Not supported");
    }
}