using EventManagementService.Application.ProcessExternalEvents.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.ProcessExternalEvents;

public static class CreateEventsServiceExtenstion
{
    public static IServiceCollection AddProcessExternalEvents(this IServiceCollection services)
    {
        services.AddScoped<IGeoCoding, GeoCoding>();
        services.AddScoped<IPubSubExternalEvents, PubSubExternalEvents>();
        services.AddScoped<ISqlExternalEvents, SqlExternalEvents>();
        return services;
    }
}