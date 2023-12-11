using UserManagementService.Application.V1.ProcessUserAchievements.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Domain.Util;
using UserManagementService.Infrastructure.PubSub;
using UserManagementService.Infrastructure.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public class CheckCulturalAndArtisticStrategy : CheckAchievementBaseStrategy
{
    public CheckCulturalAndArtisticStrategy(ISqlAchievementRepository sqlAchievementRepository) : base(
        sqlAchievementRepository)
    {
    }

    public override async Task ProcessAchievement(string userId, Category category, IEventBus eventBus = null)
    {
        var userAchievement = new List<UserAchievement>
        {
            UserAchievement.Peacock1,
            UserAchievement.Peacock2,
            UserAchievement.Peacock3
        };
        await UpdateProgress(userId, userAchievement, category, eventBus);
    }
}