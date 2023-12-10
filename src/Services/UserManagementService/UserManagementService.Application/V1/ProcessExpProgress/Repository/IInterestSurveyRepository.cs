using System.Text.Json;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<InterestSurveyRepository> _logger;

    public InterestSurveyRepository(IEventBus eventBus, IOptions<PubSub> pubsubConfig,
        ILogger<InterestSurveyRepository> logger)
    {
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<string>> GetNewlyCreatedSurveyUserList()
    {
        try
        {
            _logger.LogInformation("Retrieving newly completed interest surveys from PubSub");

            var topic = _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewAttendee];
            var userIds = (await _eventBus.PullAsync<object>(topic.TopicId, topic.ProjectId,
                    topic.SubscriptionNames[PubSubSubscriptionNames.Exp], 1000, new CancellationToken()))
                .Select(o => JsonSerializer.Serialize(o)).ToList();

            _logger.LogInformation($"Retrieved {userIds.Count} newly completed interest surveys from PubSub");

            return userIds.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to fetch new interest surveys from PubSub");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            return new List<string>();
        }
    }
}