using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;
using UserManagementService.Application.V1.ProcessUserAchievements.Repository;

namespace UserManagementService.Application.V1.ProcessUserAchievements;

public static class ProcessUserAchievementsExtension
{
    public static IServiceCollection AddProcessAchievementsExtension(this IServiceCollection services)
    {
        services.AddScoped<ISqlAchievementRepository, SqlAchievementRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICheckAchievementStrategy, CheckCulinaryAndDrinksStrategy>();
        services.AddScoped<ICheckAchievementStrategy, CheckCulturalAndArtisticStrategy>();
        services.AddScoped<ICheckAchievementStrategy, CheckHealthAndWellness>();
        services.AddScoped<ICheckAchievementStrategy, CheckLearningAndDevelopmentStrategy>();
        services.AddScoped<ICheckAchievementStrategy, CheckMusicAndPerformingArtsStrategy>();
        services.AddScoped<ICheckAchievementStrategy, CheckRecreationAndHobbiesStrategy>();
        services.AddScoped<ICheckAchievementStrategy, CheckSocialAndCommunityStrategy>();
        return services;
    }
}