using UserManagementService.Application.V1.ProcessUserAchievements.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Domain.Util;
using UserManagementService.Infrastructure.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public class CheckMusicAndPerformingArtsStrategy : CheckAchievementBaseStrategy
{
    public CheckMusicAndPerformingArtsStrategy(ISqlAchievementRepository sqlAchievementRepository) : base(
        sqlAchievementRepository)
    {
    }

    public override async Task ProcessAchievement(string userId, Category category)
    {
        var userAchievement = new List<UserAchievement>
        {
            UserAchievement.Canary1,
            UserAchievement.Canary2,
            UserAchievement.Canary3
        };
        await UpdateProgress(userId, userAchievement, category);
    }
}