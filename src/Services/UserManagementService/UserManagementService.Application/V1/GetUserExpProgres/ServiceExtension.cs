using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Application.V1.GetUserExpProgres.Repository;

namespace UserManagementService.Application.V1.GetUserExpProgres;

public static class ServiceExtension
{
    public static IServiceCollection AddGetUserExpProgress(this IServiceCollection services)
    {
        services.AddScoped<ILevelRepository, LevelRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        services.AddScoped<IUserRepository, FirestoreUserRepository>();
        
        return services;
    }
}