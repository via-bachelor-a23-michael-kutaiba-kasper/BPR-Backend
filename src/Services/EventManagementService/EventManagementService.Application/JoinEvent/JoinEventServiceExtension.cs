using EventManagementService.Application.JoinEvent.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.JoinEvent;

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