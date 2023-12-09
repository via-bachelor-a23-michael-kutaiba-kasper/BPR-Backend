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
    public AttendeesRepository(IEventBus eventBus, IOptions<PubSub> pubsubConfig)
    {
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
    }
    
    public async Task<IReadOnlyCollection<Attendance>> GetNewEventAttendees()
    {
        var topic = _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewAttendee];
        
        var newAttendees = await _eventBus.PullAsync<Attendance>(topic.TopicId, topic.ProjectId,
            _pubsubConfig.Value.SubscriptionName, 1000, new CancellationToken());

        return newAttendees.ToList();
    }
}