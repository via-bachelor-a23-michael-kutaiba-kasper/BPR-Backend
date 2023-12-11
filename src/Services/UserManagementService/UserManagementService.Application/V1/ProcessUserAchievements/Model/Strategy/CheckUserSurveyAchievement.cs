using Microsoft.Extensions.Options;
using UserManagementService.Application.V1.ProcessUserAchievements.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public class CheckUserSurveyAchievement : CheckAchievementBaseStrategy
{
    private readonly IEventBus _eventBus;
    private readonly IOptions<PubSub> _pubsubConfig;

    public CheckUserSurveyAchievement
    (
        ISqlAchievementRepository sqlAchievementRepository,
        IEventBus eventBus,
        IOptions<PubSub> pubsubConfig
    ) : base(sqlAchievementRepository, pubsubConfig)
    {
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
    }

    public override async Task ProcessAchievement(string userId, Category category, IEventBus? eventBus = null)
    {
        var userIds = await GetUserIdFromPubSub();
        if (!userIds.Contains(userId))
        {
            return;
        }
        var userAchievement = new List<UserAchievement>
        {
            UserAchievement.NewComer
        };
        await UpdateProgress(userId, userAchievement, category, eventBus);
    }


    private async Task<IReadOnlyCollection<string>> GetUserIdFromPubSub()
    {
        var userIds = await _eventBus.PullAsync<string>
        (
            _pubsubConfig.Value.Topics[PubSubTopics.NewSurvey].TopicId,
            _pubsubConfig.Value.Topics[PubSubTopics.NewSurvey].ProjectId,
            _pubsubConfig.Value.Topics[PubSubTopics.NewSurvey].SubscriptionNames[TopicSubs.UserManagementAchievements],
            10,
            new CancellationToken()
        );

        return userIds.ToList();
    }
}