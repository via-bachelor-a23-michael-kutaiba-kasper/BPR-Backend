using FirebaseAdmin.Messaging;
using Google.Cloud.Firestore.V1;
using UserManagementService.Infrastructure.Notifications.Models;
using Notification = UserManagementService.Infrastructure.Notifications.Models.Notification;

namespace UserManagementService.Infrastructure.Notifications.Strategies;

public class SendUserMessageStrategy : ISendNotificationStrategy
{
    public async Task Send(Notification notification)
    {
        var token = ((UserNotification)notification).Token;
        var firebaseMessage = new Message
        {
            Token = token,
            Notification = new FirebaseAdmin.Messaging.Notification()
            {
                Title = notification.Title,
                Body = notification.Body
            }
        };

        var defaultInstance = FirebaseMessaging.DefaultInstance;
        if (defaultInstance is null)
        {
            Firestore.CreateFirebaseApp();
            defaultInstance = FirebaseMessaging.DefaultInstance;
        }

        string response = await defaultInstance.SendAsync(firebaseMessage);
    }
}