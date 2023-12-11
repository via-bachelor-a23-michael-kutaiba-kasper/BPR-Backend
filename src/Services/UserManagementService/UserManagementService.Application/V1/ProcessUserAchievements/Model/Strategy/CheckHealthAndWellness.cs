using UserManagementService.Application.V1.ProcessUserAchievements.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Domain.Util;
using UserManagementService.Infrastructure.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public class CheckHealthAndWellness : CheckAchievementBaseStrategy
{
    public CheckHealthAndWellness(ISqlAchievementRepository sqlAchievementRepository) : base(sqlAchievementRepository)
    {
    }

    public override async Task ProcessAchievement(string userId, Category category)
    {
        var userAchievement = new List<UserAchievement>
        {
            UserAchievement.Cheetah1,
            UserAchievement.Cheetah2,
            UserAchievement.Cheetah3
        };
        await UpdateProgress(userId, userAchievement, category);
    }
}