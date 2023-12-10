using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserManagementService.Application.V1.ProcessExpProgress.Dtos;
using UserManagementService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IAttendeesRepository
{
    public Task<IReadOnlyCollection<Attendance>> GetNewEventAttendees();
}

public class AttendeesRepository : IAttendeesRepository
{
    private readonly IEventBus _eventBus;
    private readonly IOptions<PubSub> _pubsubConfig;
    private readonly ILogger<AttendeesRepository> _logger;
    public AttendeesRepository(IEventBus eventBus, IOptions<PubSub> pubsubConfig, ILogger<AttendeesRepository> logger)
    {
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
        _logger = logger;
    }
    
    public async Task<IReadOnlyCollection<Attendance>> GetNewEventAttendees()
    {
        try
        {
            _logger.LogInformation("Retrieving new attendees from PubSub");
            var topic = _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewAttendee];
        
            var newAttendees = (await _eventBus.PullAsync<Attendance>(topic.TopicId, topic.ProjectId,
                topic.SubscriptionNames[PubSubSubscriptionNames.Exp], 1000, new CancellationToken())).ToList();
        
            _logger.LogInformation($"Retrieved {newAttendees.Count} attendees from PubSub");

            return newAttendees;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to fetch new attendees from PubSub");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            return new List<Attendance>();
        }
    }
}