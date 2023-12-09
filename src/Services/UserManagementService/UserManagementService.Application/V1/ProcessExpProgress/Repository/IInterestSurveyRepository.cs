using Microsoft.Extensions.Options;
using UserManagementService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IInterestSurveyRepository
{
    Task<IReadOnlyCollection<string>> GetNewlyCreatedSurveyUserList();
}

public class InterestSurveyRepository : IInterestSurveyRepository
{
    private readonly IEventBus _eventBus;
    private readonly IOptions<PubSub> _pubsubConfig;

    public InterestSurveyRepository(IEventBus eventBus, IOptions<PubSub> pubsubConfig)
    {
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
    }

    public async Task<IReadOnlyCollection<string>> GetNewlyCreatedSurveyUserList()
    {
        var topic = _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewAttendee];
        var userIds = await _eventBus.PullAsync<string>(topic.TopicId, topic.ProjectId, _pubsubConfig.Value.SubscriptionName, 1000, new CancellationToken());

        return userIds.ToList();
    }
}