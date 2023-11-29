using EventManagementService.Application.FetchAllEvents.Repository;
using EventManagementService.Application.ProcessExternalEvents.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.FetchAllEvents;

public static class FetchAllEventsServiceExtension
{
    public static IServiceCollection AddFetchAllEvents(this IServiceCollection services)
    {
        services.AddScoped<ISqlAllEvents, SqlAllEvents>();
        
        return services;
    }
}