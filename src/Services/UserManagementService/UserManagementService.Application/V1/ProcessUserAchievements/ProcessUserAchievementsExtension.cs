using Microsoft.Extensions.DependencyInjection;

namespace UserManagementService.Application.V1.ProcessUserAchievements;

public static class ProcessUserAchievementsExtension
{
    public static IServiceCollection AddProcessAchievementsExtension(this IServiceCollection services)
    {
        return services;
    }
}