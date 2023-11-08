using EventManagementService.Application.FetchAllPublicEvents.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.FetchAllEvents;

public static class FetchAllEventsServiceExtension
{
    public static IServiceCollection AddFetchAllEvents(this IServiceCollection services)
    {
        services.AddScoped<IPubSubPublicEvents, PubSubPublicEvents>();
        services.AddScoped<ISqlPublicEvents, SqlPublicEvents>();
        services.AddScoped<IGeoCoding, GeoCoding>();
        
        return services;
    }
}