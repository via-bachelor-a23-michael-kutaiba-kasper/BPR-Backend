using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser;

public static class FetchFinishedParticipatedInEventsByUserExtension
{
    public static IServiceCollection AddFinishedParticipatedInEventsByUser(this IServiceCollection services)
    {
        services.AddScoped<ISqlEvent, SqlEvent>();
        return services;
    }
}