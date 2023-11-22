using EventManagementService.Application.CreateEvent;
using EventManagementService.Application.JoinEvent;
using EventManagementService.Application.FetchAllEvents;
using EventManagementService.Application.FetchCategories;
using EventManagementService.Application.FetchKeywords;
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
        services.AddFetchCategories();
        services.AddFetchKeywords();
        services.AddCreateEvent();
        return services;
    }
}
