using EventManagementService.Application.ScraperEvents.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.ScraperEvents;

public static class ScraperEventsServiceExtension
{
    public static IServiceCollection AddScraperEvents(this IServiceCollection services)
    {
        services.AddScoped<IPubSubScraperEvents, PubSubScraperEvents>();
        services.AddScoped<ISqlScraperEvents, SqlScraperEvents>();
        services.AddScoped<IGeoCoding, GeoCoding>();
        
        return services;
    }
}