using UserManagementService.Application.V1.ProcessUserAchievements.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Domain.Util;
using UserManagementService.Infrastructure.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public abstract class CheckAchievementBaseStrategy : ICheckAchievementStrategy
{
    private readonly ISqlAchievementRepository _sqlAchievementRepository;

    protected CheckAchievementBaseStrategy(ISqlAchievementRepository sqlAchievementRepository)
    {
        _sqlAchievementRepository = sqlAchievementRepository;
    }

    public abstract Task ProcessAchievement
    (string userId,
        Category category);


    private async Task<int> CheckAchievement
    (
        string userId,
        UserAchievement achievement,
        Category eventCategory
    )
    {
        var newProgress = 0;

        var currentUnlockedAchievemnts =
            await _sqlAchievementRepository.GetUserProgress(userId, (int)achievement, eventCategory);

        var currentProgressForAchievement =
            await _sqlAchievementRepository.GetUserAchievementsProgress(userId, (int)achievement);
        if (currentUnlockedAchievemnts.Contains((int)achievement))
        {
            return -1;
        }

        var categoryGroupAttribute =
            Attribute.GetCustomAttribute(eventCategory.GetType().GetField(eventCategory.ToString())!,
                typeof(CategoryGroupAttribute)) as CategoryGroupAttribute;
        var achievementGroupAttribute =
            Attribute.GetCustomAttribute(achievement.GetType().GetField(achievement.ToString())!,
                typeof(CategoryGroupAttribute)) as CategoryGroupAttribute;
        var categoryGroup = categoryGroupAttribute?.Group;
        var achievementGroup = achievementGroupAttribute?.Group;

        var oldProgress = currentProgressForAchievement;
        if (
            achievement == UserAchievement.NewComer || (!string.IsNullOrEmpty(categoryGroup) &&
                                                        categoryGroup == achievementGroup)
        )
        {
            // Calculate the new progress
            newProgress += oldProgress + 1;
        }
        else
        {
            return -1;
        }


        return newProgress;
    }

    protected async Task UpdateProgress(string userId, IReadOnlyCollection<UserAchievement> achievements,
        Category category)
    {
        foreach (var achievement in achievements)
        {
            var result = await CheckAchievement(userId, achievement, category);
            if (result < 0)
            {
                return;
            }

            var currentProgress = result;
            if (achievement.GetDescription().Contains('1'))
            {
                if (currentProgress >= 5)
                {
                    await _sqlAchievementRepository.InsertUserAchievement(new UserAchievementTable
                    {
                        achievement_id = (int)achievement,
                        user_id = userId,
                        unlocked_date = DateTimeOffset.UtcNow.ToUniversalTime()
                    });
                }

                await _sqlAchievementRepository.UpsertAchievementProgress(new UnlockableAchievementProgressTable
                {
                    achievement_id = (int)achievement,
                    user_id = userId,
                    date = DateTimeOffset.UtcNow.ToUniversalTime(),
                    progress = currentProgress
                });
            }

            if (achievement.GetDescription().Contains('2'))
            {
                if (currentProgress >= 20)
                {
                    await _sqlAchievementRepository.InsertUserAchievement(new UserAchievementTable
                    {
                        achievement_id = (int)achievement,
                        user_id = userId,
                        unlocked_date = DateTimeOffset.UtcNow.ToUniversalTime()
                    });
                }

                await _sqlAchievementRepository.UpsertAchievementProgress(new UnlockableAchievementProgressTable
                {
                    achievement_id = (int)achievement,
                    user_id = userId,
                    date = DateTimeOffset.UtcNow.ToUniversalTime(),
                    progress = currentProgress
                });
            }

            if (achievement.GetDescription().Contains('3'))
            {
                if (currentProgress >= 50)
                {
                    await _sqlAchievementRepository.InsertUserAchievement(new UserAchievementTable
                    {
                        achievement_id = (int)achievement,
                        user_id = userId,
                        unlocked_date = DateTimeOffset.UtcNow.ToUniversalTime()
                    });
                }

                await _sqlAchievementRepository.UpsertAchievementProgress(new UnlockableAchievementProgressTable
                {
                    achievement_id = (int)achievement,
                    user_id = userId,
                    date = DateTimeOffset.UtcNow.ToUniversalTime(),
                    progress = currentProgress
                });
            }

            if (achievement == UserAchievement.NewComer)
            {
                if (currentProgress == 0)
                {
                    return;
                }

                await _sqlAchievementRepository.InsertUserAchievement(new UserAchievementTable
                {
                    achievement_id = (int)achievement,
                    user_id = userId,
                    unlocked_date = DateTimeOffset.UtcNow.ToUniversalTime()
                });
            }
        }
    }
}