using EventManagementService.Application.FetchAllEvents.Repository;
using EventManagementService.Application.ProcessExternalEvents.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.FetchAllEvents;

public static class FetchAllEventsServiceExtension
{
    public static IServiceCollection AddFetchAllEvents(this IServiceCollection services)
    {
        services.AddScoped<IPubSubExternalEvents, PubSubExternalEvents>();
        services.AddScoped<ISqlAllEvents, SqlAllEvents>();
        services.AddScoped<IGeoCoding, GeoCoding>();
        
        return services;
    }
}