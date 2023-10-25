using EventManagementService.Application.FetchAllPublicEvents.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.FetchAllPublicEvents;

public static class ScraperEventsServiceExtension
{
    public static IServiceCollection AddScraperEvents(this IServiceCollection services)
    {
        services.AddScoped<IPubSubPublicEvents, PubSubPublicEvents>();
        services.AddScoped<ISqlPublicEvents, SqlPublicEvents>();
        services.AddScoped<IGeoCoding, GeoCoding>();
        
        return services;
    }
}