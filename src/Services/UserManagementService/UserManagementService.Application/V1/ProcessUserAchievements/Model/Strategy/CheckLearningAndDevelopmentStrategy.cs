using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public class CheckLearningAndDevelopmentStrategy : CheckAchievementBaseStrategy
{
    public override IDictionary<string, IReadOnlyCollection<UserAchievement>> CheckAchievement
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var c1 = DoCheckAchievement
        (
            unlockedAchievements,
            UserAchievement.Owl1,
            categoryCounts,
            AchievementsRequirements.Tier1
        );
        var c2 = DoCheckAchievement
        (
            unlockedAchievements,
            UserAchievement.Owl2,
            categoryCounts,
            AchievementsRequirements.Tier2
        );
        var c3 = DoCheckAchievement
        (
            unlockedAchievements,
            UserAchievement.Owl3,
            categoryCounts,
            AchievementsRequirements.Tier3
        );
        
        var results = c1.Concat(c2).Concat(c3).ToDictionary(pair => pair.Key, pair => pair.Value);
        return results;
    }
}