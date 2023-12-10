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

        var currentProgress =
            await _sqlAchievementRepository.GetUserProgress(userId, (int)achievement, eventCategory);
        if (currentProgress.Contains((int)achievement))
        {
            return -1;
        }

        var categoryGroupAttribute =
            Attribute.GetCustomAttribute(eventCategory.GetType().GetField(eventCategory.ToString())!,
                typeof(CategoryGroupAttribute)) as CategoryGroupAttribute;
        var categoryGroup = categoryGroupAttribute?.Group;

        var oldProgress = await _sqlAchievementRepository.GetProgressForAnAchievement(userId, (int)achievement);
        /*if (!string.IsNullOrEmpty(categoryGroup)) //Todo: not sure about this part
        {
            // Calculate the new progress
            /*newProgress++;
            newProgress += oldProgress;#1#
            oldProgress++;
        }*/


        return oldProgress;
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


            var currentProgress =
                await _sqlAchievementRepository.GetProgressForAnAchievement(userId, (int)achievement) + result;
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