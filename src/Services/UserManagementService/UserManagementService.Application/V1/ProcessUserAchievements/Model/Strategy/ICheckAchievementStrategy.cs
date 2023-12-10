using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public interface ICheckAchievementStrategy
{
    Task ProcessAchievement(string userId, Category category);
}