using System.Text.Json;
using EventManagementService.Infrastructure.Notifications.Models;
using FirebaseAdmin.Messaging;
using Notification = EventManagementService.Infrastructure.Notifications.Models.Notification;

namespace EventManagementService.Infrastructure.Notifications.Strategies;

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