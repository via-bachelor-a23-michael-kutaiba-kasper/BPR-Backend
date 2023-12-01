using EventManagementService.Application.V1.JoinEvent.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.V1.JoinEvent;

public static class JoinEventServiceExtension
{
    public static IServiceCollection AddJoinEvent(this IServiceCollection collection)
    {
        collection.AddScoped<IEventRepository, EventRepository>();
        collection.AddScoped<IUserRepository, UserRepository>();
        collection.AddScoped<IInvitationRepository, InvitationRepository>();
        
        return collection;
    }
}