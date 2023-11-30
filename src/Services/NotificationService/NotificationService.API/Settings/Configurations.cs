using NotificationService.Infrastructure.AppSettings;

namespace NotificationService.API.Settings;

internal static class Configurations
{
    internal static IServiceCollection AddSettingsConfigurations(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<PubSub>(configuration.GetSection("PubSub"));
        return services;
    }
}