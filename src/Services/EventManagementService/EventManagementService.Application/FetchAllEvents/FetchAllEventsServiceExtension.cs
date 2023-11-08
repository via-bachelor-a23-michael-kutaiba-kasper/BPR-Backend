using EventManagementService.Application.CreateEvents.Repository;
using EventManagementService.Application.FetchAllEvents.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.FetchAllEvents;

public static class FetchAllEventsServiceExtension
{
    public static IServiceCollection AddFetchAllEvents(this IServiceCollection services)
    {
        services.AddScoped<IPubSubPublicEvents, PubSubPublicEvents>();
        services.AddScoped<ISqlAllEvents, SqlAllEvents>();
        services.AddScoped<IGeoCoding, GeoCoding>();
        
        return services;
    }
}