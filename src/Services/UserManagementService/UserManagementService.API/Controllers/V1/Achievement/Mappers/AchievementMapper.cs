using UserManagementService.API.Controllers.V1.Achievement.Dtos;
using UserManagementService.Application.V1.FetchAllAchievements.Model;
using UserManagementService.Domain.Models;

namespace UserManagementService.API.Controllers.V1.Achievement.Mappers;

internal static class AchievementMapper
{
    internal static IReadOnlyCollection<AchievementDto> FromDomainToDtoUserAchievementListMapper(
        IReadOnlyCollection<Domain.Models.Achievement> achievements)
    {
        var userAchievements = new List<AchievementDto>();
        if (!achievements.Any())
        {
            return new List<AchievementDto>();
        }

        foreach (var ac in achievements)
        {
            var requirement = 0;
            if (ac.Name.Contains('1'))
            {
                requirement = AchievementsRequirements.Tier1;
            }

            else if (ac.Name.Contains('2'))
            {
                requirement = AchievementsRequirements.Tier2;
            }

            else if (ac.Name.Contains('3'))
            {
                requirement = AchievementsRequirements.Tier3;
            }
            else
            {
                requirement = 1;
            }

            userAchievements.Add(new AchievementDto
            {
                Description = ac.Description,
                Name = ac.Name,
                Icon = ac.Icon,
                ExpReward = ac.ExpReward,
                UnlockDate = ac.UnlockDate,
                Progress = ac.Progress,
                Requirement = requirement
            });
        }

        return userAchievements;
    }

    internal static IReadOnlyCollection<AllAchievementsDto> FromDomainToDtoAllAchievementsListMapper(
        IReadOnlyCollection<AchievementTable> achievements)
    {
        var dtoAchievements = new List<AllAchievementsDto>();
        if (!achievements.Any())
        {
            return dtoAchievements;
        }

        foreach (var ac in achievements)
        {
            var requirement = 0;
            if (ac.name.Contains('1'))
            {
                requirement = AchievementsRequirements.Tier1;
            }

            else if (ac.name.Contains('2'))
            {
                requirement = AchievementsRequirements.Tier2;
            }

            else if (ac.name.Contains('3'))
            {
                requirement = AchievementsRequirements.Tier3;
            }
            else
            {
                requirement = 1;
            }

            dtoAchievements.Add(new AllAchievementsDto
            {
                Description = ac.description,
                Icon = ac.icon,
                Name = ac.name,
                Requirement = requirement,
                Reward = ac.reward
            });
        }

        return dtoAchievements;
    }
}