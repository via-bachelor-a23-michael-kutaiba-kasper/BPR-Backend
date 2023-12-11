using Microsoft.Extensions.Options;
using UserManagementService.Application.V1.ProcessExpProgress.Dtos;
using UserManagementService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IAchievementsRepository
{
    Task<IEnumerable<AchievementUnlocked>> GetNewlyUnlockedAchievements();
}

public class AchievementsRepository : IAchievementsRepository
{
    private readonly IEventBus _eventBus;
    private readonly IOptions<PubSub> _pubsubConfig;

    public AchievementsRepository(IEventBus eventBus, IOptions<PubSub> pubsubConfig)
    {
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
    }

    public async Task<IEnumerable<AchievementUnlocked>> GetNewlyUnlockedAchievements()
    {
        var topic = _pubsubConfig.Value.Topics[PubSubTopics.VibeverseAchievementsNewAchievement];
        var achievements = await _eventBus.PullAsync<AchievementUnlocked>(
            topic.TopicId, topic.ProjectId, topic.SubscriptionNames[PubSubSubscriptionNames.Exp], 1000,
            new CancellationToken()
        );
        
        return achievements;
    }
}