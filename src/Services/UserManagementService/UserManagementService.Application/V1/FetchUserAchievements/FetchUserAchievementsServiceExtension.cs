using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Application.V1.FetchUserAchievements.Repository;

namespace UserManagementService.Application.V1.FetchUserAchievements;

public static class FetchUserAchievementsServiceExtension
{
    public static IServiceCollection AddFetchUserAchievements(this IServiceCollection services)
    {
        services.AddScoped<ISqlUserAchievementsRepository, SqlUserAchievementsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}