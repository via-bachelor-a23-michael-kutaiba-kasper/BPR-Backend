using UserManagementService.Domain.Models;

namespace UserManagementService.Domain.Util;

public static class EnumCategoryGroupHelper
{
    public static bool AreEnumsInSameCategoryGroup(Enum value1, Enum value2)
    {
        var groupAttribute1 = GetCategoryGroupAttribute(value1);
        var groupAttribute2 = GetCategoryGroupAttribute(value2);

        if (groupAttribute1 != null && groupAttribute2 != null)
        {
            return groupAttribute1.Group == groupAttribute2.Group;
        }
        
        return false;
    }

    public static CategoryGroupAttribute? GetCategoryGroupAttribute(Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        if (fieldInfo == null) return null;
        var attributes = fieldInfo.GetCustomAttributes(typeof(CategoryGroupAttribute), false);
        if (attributes.Length > 0)
        {
            return (CategoryGroupAttribute)attributes[0];
        }

        return null;
    }
    
    private static readonly Dictionary<string, List<UserAchievement>> CategoryToAchievementsMap = new()
    {
        { "Music and Performing Arts", new List<UserAchievement> { UserAchievement.Canary1, UserAchievement.Canary2, UserAchievement.Canary3 } },
        { "Learning and Development", new List<UserAchievement> { UserAchievement.Owl1, UserAchievement.Owl2, UserAchievement.Owl3 } },
        { "Cultural and Artistic", new List<UserAchievement> { UserAchievement.Peacock1, UserAchievement.Peacock2, UserAchievement.Peacock3 } },
        { "Culinary and Drinks", new List<UserAchievement> { UserAchievement.Bear1, UserAchievement.Bear2, UserAchievement.Bear3 } },
        { "Social and Community", new List<UserAchievement> { UserAchievement.Butterfly1, UserAchievement.Butterfly2, UserAchievement.Butterfly3 } },
        { "Health and Wellness", new List<UserAchievement> { UserAchievement.Cheetah1, UserAchievement.Cheetah2, UserAchievement.Cheetah3 } },
        { "Recreation and Hobbies", new List<UserAchievement> { UserAchievement.Monkey1, UserAchievement.Monkey2, UserAchievement.Monkey3 } },
        { "New comer", new List<UserAchievement> { UserAchievement.NewComer} }
    };

    public static List<UserAchievement> GetAchievementsForCategoryGroup(string categoryGroup)
    {
        if (CategoryToAchievementsMap.TryGetValue(categoryGroup, out var achievements))
        {
            return achievements;
        }

        return new List<UserAchievement>();
    }
}