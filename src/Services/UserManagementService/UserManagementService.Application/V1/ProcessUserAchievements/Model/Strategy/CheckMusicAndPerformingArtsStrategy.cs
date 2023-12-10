using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public class CheckMusicAndPerformingArtsStrategy : CheckAchievementBaseStrategy
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
            UserAchievement.Canary1,
            categoryCounts,
            AchievementsRequirements.Tier1
        );
        var c2 = DoCheckAchievement
        (
            unlockedAchievements,
            UserAchievement.Canary2,
            categoryCounts,
            AchievementsRequirements.Tier2
        );
        var c3 = DoCheckAchievement
        (
            unlockedAchievements,
            UserAchievement.Canary3,
            categoryCounts,
            AchievementsRequirements.Tier3
        );
        
        var alreadyUnlocked = c1[AchievementsTypes.AlreadyUnlocked].Concat(c2[AchievementsTypes.AlreadyUnlocked])
            .Concat(c3[AchievementsTypes.AlreadyUnlocked]).ToList();
        var inProgress = c1[AchievementsTypes.InProgress].Concat(c2[AchievementsTypes.InProgress])
            .Concat(c3[AchievementsTypes.InProgress]).ToList();
        var unlocked = c1[AchievementsTypes.Unlocked].Concat(c2[AchievementsTypes.Unlocked])
            .Concat(c3[AchievementsTypes.Unlocked]).ToList();
        var results = new Dictionary<string, IReadOnlyCollection<UserAchievement>>();
        results.Add(AchievementsTypes.AlreadyUnlocked, alreadyUnlocked.ToList());
        results.Add(AchievementsTypes.InProgress, inProgress.ToList());
        results.Add(AchievementsTypes.Unlocked, unlocked.ToList());
        return results;
    }
}