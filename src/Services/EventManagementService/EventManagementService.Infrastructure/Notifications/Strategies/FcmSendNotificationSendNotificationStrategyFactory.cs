using EventManagementService.Infrastructure.Notifications.Models;
using FirebaseAdmin.Messaging;
using Notification = EventManagementService.Infrastructure.Notifications.Models.Notification;

namespace EventManagementService.Infrastructure.Notifications.Strategies;

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