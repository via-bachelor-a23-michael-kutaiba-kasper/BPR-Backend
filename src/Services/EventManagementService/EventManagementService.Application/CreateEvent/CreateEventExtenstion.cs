using EventManagementService.Application.CreateEvent.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.CreateEvent;

public static class CreateEventExtenstion
{
    public static IServiceCollection AddCreateEvent(this IServiceCollection services)
    {
        services.AddScoped<ISqlCreateEvent, SqlCreateEvent>();
        return services;
    }
}