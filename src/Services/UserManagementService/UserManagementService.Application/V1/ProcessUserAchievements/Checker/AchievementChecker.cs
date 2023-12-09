using UserManagementService.Application.V1.ProcessUserAchievements.Model;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Domain.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Checker;

internal static class AchievementChecker
{
    internal static IReadOnlyCollection<UserAchievement>? CheckMusicAndPerformingArts
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var newAchievements = new List<UserAchievement>();
        var c1 = CheckAchievement(unlockedAchievements, UserAchievement.Canary1, categoryCounts, 5);
        var c2 = CheckAchievement(unlockedAchievements, UserAchievement.Canary2, categoryCounts, 20);
        var c3 = CheckAchievement(unlockedAchievements, UserAchievement.Canary3, categoryCounts, 50);
        if (c1 != null)
        {
            newAchievements.AddRange(c1);
        }

        if (c2 != null)
        {
            newAchievements.AddRange(c2);
        }

        if (c3 != null)
        {
            newAchievements.AddRange(c3);
        }

        return newAchievements;
    }

    internal static IReadOnlyCollection<UserAchievement>? CheckLearningAndDevelopment
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var newAchievements = new List<UserAchievement>();
        var c1 = CheckAchievement(unlockedAchievements, UserAchievement.Owl1, categoryCounts, 5);
        var c2 = CheckAchievement(unlockedAchievements, UserAchievement.Owl2, categoryCounts, 20);
        var c3 = CheckAchievement(unlockedAchievements, UserAchievement.Owl3, categoryCounts, 50);
        if (c1 != null)
        {
            newAchievements.AddRange(c1);
        }

        if (c2 != null)
        {
            newAchievements.AddRange(c2);
        }

        if (c3 != null)
        {
            newAchievements.AddRange(c3);
        }

        return newAchievements;
    }

    internal static IReadOnlyCollection<UserAchievement>? CheckCulturalAndArtistic
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var newAchievements = new List<UserAchievement>();
        var c1 = CheckAchievement(unlockedAchievements, UserAchievement.Peacock1, categoryCounts, 5);
        var c2 = CheckAchievement(unlockedAchievements, UserAchievement.Peacock2, categoryCounts, 20);
        var c3 = CheckAchievement(unlockedAchievements, UserAchievement.Peacock3, categoryCounts, 50);
        if (c1 != null)
        {
            newAchievements.AddRange(c1);
        }

        if (c2 != null)
        {
            newAchievements.AddRange(c2);
        }

        if (c3 != null)
        {
            newAchievements.AddRange(c3);
        }

        return newAchievements;
    }

    internal static IReadOnlyCollection<UserAchievement>? CheckCulinaryAndDrinks
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var newAchievements = new List<UserAchievement>();
        var c1 = CheckAchievement(unlockedAchievements, UserAchievement.Bear1, categoryCounts, 5);
        var c2 = CheckAchievement(unlockedAchievements, UserAchievement.Bear2, categoryCounts, 20);
        var c3 = CheckAchievement(unlockedAchievements, UserAchievement.Bear3, categoryCounts, 50);
        if (c1 != null)
        {
            newAchievements.AddRange(c1);
        }

        if (c2 != null)
        {
            newAchievements.AddRange(c2);
        }

        if (c3 != null)
        {
            newAchievements.AddRange(c3);
        }

        return newAchievements;
    }

    internal static IReadOnlyCollection<UserAchievement>? CheckSocialAndCommunity
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var newAchievements = new List<UserAchievement>();
        var c1 = CheckAchievement(unlockedAchievements, UserAchievement.Butterfly1, categoryCounts, 5);
        var c2 = CheckAchievement(unlockedAchievements, UserAchievement.Butterfly2, categoryCounts, 20);
        var c3 = CheckAchievement(unlockedAchievements, UserAchievement.Butterfly3, categoryCounts, 50);
        if (c1 != null)
        {
            newAchievements.AddRange(c1);
        }

        if (c2 != null)
        {
            newAchievements.AddRange(c2);
        }

        if (c3 != null)
        {
            newAchievements.AddRange(c3);
        }

        return newAchievements;
    }

    internal static IReadOnlyCollection<UserAchievement>? CheckHealthAndWellness
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var newAchievements = new List<UserAchievement>();
        var c1 = CheckAchievement(unlockedAchievements, UserAchievement.Cheetah1, categoryCounts, 5);
        var c2 = CheckAchievement(unlockedAchievements, UserAchievement.Cheetah2, categoryCounts, 20);
        var c3 = CheckAchievement(unlockedAchievements, UserAchievement.Cheetah3, categoryCounts, 50);
        if (c1 != null)
        {
            newAchievements.AddRange(c1);
        }

        if (c2 != null)
        {
            newAchievements.AddRange(c2);
        }

        if (c3 != null)
        {
            newAchievements.AddRange(c3);
        }

        return newAchievements;
    }

    internal static IReadOnlyCollection<UserAchievement>? CheckRecreationAndHobbies
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var newAchievements = new List<UserAchievement>();
        var c1 = CheckAchievement(unlockedAchievements, UserAchievement.Monkey1, categoryCounts, 5);
        var c2 = CheckAchievement(unlockedAchievements, UserAchievement.Monkey2, categoryCounts, 20);
        var c3 = CheckAchievement(unlockedAchievements, UserAchievement.Monkey3, categoryCounts, 50);
        if (c1 != null)
        {
            newAchievements.AddRange(c1);
        }

        if (c2 != null)
        {
            newAchievements.AddRange(c2);
        }

        if (c3 != null)
        {
            newAchievements.AddRange(c3);
        }

        return newAchievements;
    }

    private static IReadOnlyCollection<UserAchievement>? CheckAchievement
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


    internal static IReadOnlyCollection<UserAchievement>? UnlockLowerTierAchievements
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
        return achievement switch
        {
            UserAchievement.Canary1 => AchievementsRequirements.Tier1,
            UserAchievement.Canary2 => AchievementsRequirements.Tier2,
            UserAchievement.Canary3 => AchievementsRequirements.Tier3,
            _ => 0
        };
    }
}