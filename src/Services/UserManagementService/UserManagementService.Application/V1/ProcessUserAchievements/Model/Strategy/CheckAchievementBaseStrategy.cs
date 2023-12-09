using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Domain.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public abstract class CheckAchievementBaseStrategy : ICheckAchievementStrategy
{
    public abstract IReadOnlyCollection<UserAchievement> CheckAchievement
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    );


    protected static IReadOnlyCollection<UserAchievement>? DoCheckAchievement
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        UserAchievement achievement,
        Dictionary<Category, int> categoryCounts,
        int requiredCount
    )
    {
        if (unlockedAchievements == null && unlockedAchievements!.Any(a => a.achievement_id == (int)achievement))
            return null;
        var newAchievements = new List<UserAchievement>();
        var categoryAttribute = EnumCategoryGroupHelper.GetCategoryGroupAttribute(achievement);

        if (categoryAttribute == null) return null;
        var category = (Category)Enum.Parse(typeof(Category), achievement.ToString());

        if (!categoryCounts.TryGetValue(category, out var userCount) || userCount < requiredCount) return null;
        var lowerTierAchievements = UnlockLowerTierAchievements(achievement, categoryCounts);
        if (lowerTierAchievements != null)
        {
            newAchievements.AddRange(lowerTierAchievements);
        }

        newAchievements.Add(achievement);
        return newAchievements;
    }


    private static IReadOnlyCollection<UserAchievement>? UnlockLowerTierAchievements
    (
        UserAchievement achievement,
        Dictionary<Category, int> categoryCounts
    )
    {
        var achievements = new List<UserAchievement>();
        for (var i = 1; i < (int)achievement; i++)
        {
            var lowerTierAchievement = (UserAchievement)i;
            var lowerTierRequiredCount = GetRequiredCountForAchievement(lowerTierAchievement);

            if (categoryCounts.All(pair => pair.Value >= lowerTierRequiredCount))
            {
                achievements.Add((UserAchievement)i);
            }
        }

        return achievements;
    }

    private static int GetRequiredCountForAchievement(UserAchievement achievement)
    {
        // all achievement have the same tier so the use of canary here is valid for each achievement
        return achievement switch
        {
            UserAchievement.Canary1 => AchievementsRequirements.Tier1,
            UserAchievement.Canary2 => AchievementsRequirements.Tier2,
            UserAchievement.Canary3 => AchievementsRequirements.Tier3,
            _ => 0
        };
    }
}