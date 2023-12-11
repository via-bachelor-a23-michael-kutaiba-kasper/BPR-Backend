using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Application.V1.FetchAllAchievements.Repository;

namespace UserManagementService.Application.V1.FetchAllAchievements;

public static class FetchAllAchievementsExtension
{
    public static IServiceCollection AddFetchAllAchievements(this IServiceCollection services)
    {
        services.AddScoped<ISqlAchievementsRepository, SqlAchievementsRepository>();
        return services;
    }
}