using RecommendationService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.AppSettings;

namespace UserManagementService.API.Settings;

internal static class Configurations
{
    internal static IServiceCollection AddSettingsConfigurations(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));
        services.Configure<PubSub>(configuration.GetSection("PubSub"));
        services.Configure<Gateway>(configuration.GetSection("Gateway"));
        return services;
    }
}