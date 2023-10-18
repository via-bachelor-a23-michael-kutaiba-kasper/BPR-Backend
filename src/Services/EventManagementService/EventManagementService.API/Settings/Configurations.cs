using EventManagementService.Infrastructure.AppSettings;

namespace EventManagementService.API.Settings;

internal static class Configurations
{
    internal static IServiceCollection AddSettingsConfigurations(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<ConnectionStrings>(configuration.GetSection("Postgres"));
        return services;
    }
}