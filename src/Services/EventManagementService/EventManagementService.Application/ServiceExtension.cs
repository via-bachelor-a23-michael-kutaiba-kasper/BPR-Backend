using EventManagementService.Application.CreateEvents;
using EventManagementService.Application.FetchAllEvents;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application;
public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddFetchAllEvents();
        services.AddCreateEvents();
        return services;
    }
}
