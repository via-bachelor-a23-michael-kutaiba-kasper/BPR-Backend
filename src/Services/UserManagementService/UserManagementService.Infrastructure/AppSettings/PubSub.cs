namespace UserManagementService.Infrastructure.AppSettings;

public class PubSub
{
    public Topic[] Topics { get; set; }
}

public class Topic
{
    public string ProjectId { get; set; }
    public string TopicId { get; set; }
    public string[] SubscriptionNames { get; set; }
}