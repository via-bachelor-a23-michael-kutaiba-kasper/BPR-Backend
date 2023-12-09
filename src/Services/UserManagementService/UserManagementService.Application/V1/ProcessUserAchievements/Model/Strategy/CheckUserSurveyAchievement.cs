using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public class CheckUserSurveyAchievement : CheckAchievementBaseStrategy
{
    public override IReadOnlyCollection<UserAchievement> CheckAchievement
    (
        IReadOnlyCollection<UserAchievementJoinTable>? unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var achievement = new List<UserAchievement>();
        if (unlockedAchievements == null) return achievement;
        achievement.AddRange
        (
            from ac in unlockedAchievements
            where ac.achievement_id == (int)UserAchievement.NewComer
            select UserAchievement.NewComer
        );

        return achievement;
    }
}