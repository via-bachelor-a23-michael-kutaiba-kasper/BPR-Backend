namespace NotificationService.Infrastructure.AppSettings;

public class Topic
{
    public string ProjectId { get; set; }
    public string TopicId { get; set; }
}

public class PubSub
{
    public Topic[] Topics { get; set; }
    public string SubscriptionName { get; set; }
}