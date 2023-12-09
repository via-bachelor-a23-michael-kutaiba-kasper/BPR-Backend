using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Domain.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public abstract class CheckAchievementBaseStrategy : ICheckAchievementStrategy
{
    public abstract IDictionary<string, IReadOnlyCollection<UserAchievement>> CheckAchievement
    (IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts);


    protected static IDictionary<string, IReadOnlyCollection<UserAchievement>> DoCheckAchievement
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        UserAchievement achievement,
        Dictionary<Category, int> categoryCounts,
        int requiredCount
    )
    {
        var results = new Dictionary<string, IReadOnlyCollection<UserAchievement>>();
        var alreadyUnlocked = new List<UserAchievement>();
        var unlocked = new List<UserAchievement>();
        var inProgress = new List<UserAchievement>();

        if (unlockedAchievements != null)
        {
            alreadyUnlocked.AddRange
            (
                from ac in unlockedAchievements
                where ac.achievement_id == (int)achievement
                select (UserAchievement)ac.achievement_id
            );
        }

        var categoryAttribute = EnumCategoryGroupHelper.GetCategoryGroupAttribute(achievement);

        foreach
        (
            var e in from e in categoryCounts
            let achGroup = EnumCategoryGroupHelper.AreEnumsInSameCategoryGroup(achievement, e.Key)
            where achGroup
            select e
        )
        {
            if (e.Value < requiredCount)
            {
                inProgress.Add(achievement);
            }

            if (e.Value >= requiredCount)
            {
                unlocked.Add(achievement);
            }
        }

        var lowerTierAchievements =
            UnlockLowerTierAchievements(achievement, categoryCounts) ?? new List<UserAchievement>();

        unlocked.AddRange(lowerTierAchievements);

        results.Add(AchievementsTypes.AlreadyUnlocked, alreadyUnlocked);
        results.Add(AchievementsTypes.Unlocked, unlocked);
        results.Add(AchievementsTypes.InProgress, inProgress);

        return results;
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