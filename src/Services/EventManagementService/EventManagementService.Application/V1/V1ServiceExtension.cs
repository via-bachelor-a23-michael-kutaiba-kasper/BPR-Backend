using EventManagementService.Application.V1.CreateEvent;
using EventManagementService.Application.V1.FetchAllEvents;
using EventManagementService.Application.V1.FetchCategories;
using EventManagementService.Application.V1.FetchEventById;
using EventManagementService.Application.V1.FetchKeywords;
using EventManagementService.Application.V1.JoinEvent;
using EventManagementService.Application.V1.ProcessExternalEvents;
using EventManagementService.Application.V1.ReviewEvent;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.V1;

public static class V1ServiceExtension
{
    public static IServiceCollection AddV1Services(this IServiceCollection services)
    {
        services.AddFetchAllEvents();
        services.AddProcessExternalEvents();
        services.AddJoinEvent();
        services.AddFetchCategories();
        services.AddFetchKeywords();
        services.AddCreateEvent();
        services.AddFetchEventById();
        services.AddReviewEventServices();
        return services;
    }
}