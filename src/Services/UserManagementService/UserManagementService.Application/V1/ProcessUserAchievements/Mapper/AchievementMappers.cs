using UserManagementService.Application.V1.ProcessUserAchievements.Model;
using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Mapper;

internal static class AchievementMappers
{
    internal static IReadOnlyCollection<Achievement>? FromDtoToDomainAchievements(
        IReadOnlyCollection<UserAchievementJoinTable>? userAchievementTables)
    {
        return userAchievementTables?.Select(ua => new Achievement
            {
                Name = ua.name,
                Description = ua.description,
                Icon = ua.icon,
                ExpReward = ua.reward,
                UnlockDate = ua.unlocked_date
            })
            .ToList();
    }
}