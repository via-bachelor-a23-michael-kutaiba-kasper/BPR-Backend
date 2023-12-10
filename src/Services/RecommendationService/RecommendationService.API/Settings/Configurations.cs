using RecommendationService.Infrastructure.AppSettings;

namespace RecommendationService.API.Settings;

internal static class Configurations
{
    internal static IServiceCollection AddAppSettingsConfigurations(this IServiceCollection collection,
        ConfigurationManager configurationManager)
    {
        collection.Configure<Gateway>(configurationManager.GetSection("Gateway"));
        collection.Configure<PubSub>(configurationManager.GetSection("PubSub"));

        return collection;
    }
}