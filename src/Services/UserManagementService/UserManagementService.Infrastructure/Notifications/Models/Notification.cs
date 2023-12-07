namespace UserManagementService.Infrastructure.Notifications.Models;

public class Notification
{
    public string Title { get; set; }
    public string Body { get; set; }
}

public class BroadcastNotification : Notification
{
    public IReadOnlyCollection<string> Tokens { get; set; } = new List<string>();
}

public class UserNotification : Notification
{
    public string Token { get; set; } = "";
}

public class ConditionalNotification : Notification
{
    public string Condition { get; set; } = "";
}