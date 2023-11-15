using EventManagementService.Application.FetchAllPublicEvents;
using EventManagementService.Application.JoinEvent;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application;
public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScraperEvents();
        services.AddJoinEvent();
        return services;
    }
}
