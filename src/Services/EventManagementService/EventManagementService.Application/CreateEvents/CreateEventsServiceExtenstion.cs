using EventManagementService.Application.CreateEvents.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.CreateEvents;

public static class CreateEventsServiceExtenstion
{
    public static IServiceCollection AddCreateEvents(this IServiceCollection services)
    {
        services.AddScoped<IGeoCoding, GeoCoding>();
        services.AddScoped<IPubSubPublicEvents, PubSubPublicEvents>();
        services.AddScoped<ISqlCreateEvents, SqlCreateEvents>();
        return services;
    }
}