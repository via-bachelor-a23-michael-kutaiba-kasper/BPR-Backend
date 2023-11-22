using EventManagementService.Application.JoinEvent;
using EventManagementService.Application.FetchAllEvents;
using EventManagementService.Application.ProcessExternalEvents;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application;
public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddFetchAllEvents();
        services.AddProcessExternalEvents();
        services.AddJoinEvent();
        return services;
    }
}
