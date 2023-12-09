using Microsoft.Extensions.Options;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public class CheckUserSurveyAchievement : CheckAchievementBaseStrategy
{
    private readonly IEventBus _eventBus;
    private readonly IOptions<PubSub> _pubsubConfig;

    public CheckUserSurveyAchievement(IEventBus eventBus, IOptions<PubSub> pubsubConfig)
    {
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
    }

    public override IReadOnlyCollection<UserAchievement> CheckAchievement
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var achievements = new List<UserAchievement>();

        var ids = GetUserIdFromPubSub().Result;

        if
        (
            unlockedAchievements == null || (unlockedAchievements.Any(ac =>
                ac.achievement_id == (int)UserAchievement.NewComer || ids.Any(id => id != ac.user_id)))
        ) return achievements;

        achievements.Add(UserAchievement.NewComer);
        return achievements;
    }

    private async Task<IReadOnlyCollection<string>> GetUserIdFromPubSub()
    {
        var userIds = await _eventBus.PullAsync<string>
        (
            _pubsubConfig.Value.Topics[PubSubTopics.NewSurvey].TopicId,
            _pubsubConfig.Value.Topics[PubSubTopics.NewSurvey].ProjectId,
            _pubsubConfig.Value.SubscriptionName,
            10,
            new CancellationToken()
        );

        return userIds.ToList();
    }
}