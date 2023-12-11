using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Application.V1.FetchUserAchievements;
using UserManagementService.Application.V1.GetUserExpProgres;
using UserManagementService.Application.V1.ProcessExpProgress;
using UserManagementService.Application.V1.ProcessUserAchievements;
using UserManagementService.Application.V1.ProcessUserAchievements.Repository;

namespace UserManagementService.Application.V1;

public static class V1ServiceExtension
{
    public static IServiceCollection AddV1ServiceCollection(this IServiceCollection services)
    {
        services.AddProcessExpProgress();
        services.AddGetUserExpProgress();
        services.AddProcessAchievementsExtension();
        services.AddFetchUserAchievements();
        return services;
    }
}