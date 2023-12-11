using UserManagementService.Application.V1.ProcessUserAchievements.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Infrastructure.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public class CheckCulinaryAndDrinksStrategy : CheckAchievementBaseStrategy
{
    public CheckCulinaryAndDrinksStrategy(ISqlAchievementRepository sqlAchievementRepository) : base(
        sqlAchievementRepository)
    {
    }

    public override async Task ProcessAchievement(string userId, Category category)
    {
        var userAchievement = new List<UserAchievement>
        {
            UserAchievement.Bear1,
            UserAchievement.Bear2,
            UserAchievement.Bear3
        };
        await UpdateProgress(userId, userAchievement, category);
    }
}