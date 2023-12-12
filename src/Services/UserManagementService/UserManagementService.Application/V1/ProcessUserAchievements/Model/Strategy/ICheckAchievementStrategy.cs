using UserManagementService.Domain.Models.Events;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;

public interface ICheckAchievementStrategy
{
    Task ProcessAchievement(string userId, Category category, IEventBus? _eventBus = null);
}