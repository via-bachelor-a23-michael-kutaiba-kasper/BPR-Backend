using EventManagementService.Application.V1.FetchAllEvents.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.V1.FetchAllEvents;

public static class FetchAllEventsServiceExtension
{
    public static IServiceCollection AddFetchAllEvents(this IServiceCollection services)
    {
        services.AddScoped<ISqlAllEvents, SqlAllEvents>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}