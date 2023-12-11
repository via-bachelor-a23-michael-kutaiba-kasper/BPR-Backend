using UserManagementService.API.Controllers.V1.Achievement.Dtos;

namespace UserManagementService.API.Controllers.V1.Achievement.Mappers;

internal static class UserAchievementMapper
{
    internal static IReadOnlyCollection<AchievementDto> FromDomainToDtoMapper(
        IReadOnlyCollection<Domain.Models.Achievement> achievements)
    {
        if (!achievements.Any())
        {
            return new List<AchievementDto>();
        }

        return achievements.Select(ac => new AchievementDto
            {
                Description = ac.Description,
                Name = ac.Name,
                Icon = ac.Icon,
                ExpReward = ac.ExpReward,
                UnlockDate = ac.UnlockDate
            })
            .ToList();
    }
}