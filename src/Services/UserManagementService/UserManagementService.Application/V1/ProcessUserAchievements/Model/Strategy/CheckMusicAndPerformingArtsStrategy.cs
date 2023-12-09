using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public class CheckMusicAndPerformingArtsStrategy : CheckAchievementBaseStrategy
{
    public override IReadOnlyCollection<UserAchievement> CheckAchievement
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var newAchievements = new List<UserAchievement>();
        var c1 = DoCheckAchievement
        (
            unlockedAchievements, 
            UserAchievement.Canary1,
            categoryCounts,
            AchievementsRequirements.Tier1
        ) ?? new List<UserAchievement>();
        var c2 = DoCheckAchievement
        (
            unlockedAchievements,
            UserAchievement.Canary2,
            categoryCounts,
            AchievementsRequirements.Tier2
        ) ?? new List<UserAchievement>();
        var c3 = DoCheckAchievement
        (
            unlockedAchievements,
            UserAchievement.Canary3,
            categoryCounts,
            AchievementsRequirements.Tier3
        ) ?? new List<UserAchievement>();
        
        newAchievements.AddRange(c1);
        newAchievements.AddRange(c2);
        newAchievements.AddRange(c3);
        return newAchievements;
    }
}