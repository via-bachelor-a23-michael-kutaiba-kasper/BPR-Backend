using EventManagementService.Application.V1.CreateEvent.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.V1.CreateEvent;

public static class CreateEventExtenstion
{
    public static IServiceCollection AddCreateEvent(this IServiceCollection services)
    {
        services.AddScoped<ISqlCreateEvent, SqlCreateEvent>();
        services.AddScoped<IFirebaseUser, FirebaseUser>();
        return services;
    }
}