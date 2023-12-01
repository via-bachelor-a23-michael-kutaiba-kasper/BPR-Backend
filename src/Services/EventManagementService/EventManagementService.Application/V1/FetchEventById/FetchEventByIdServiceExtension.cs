using EventManagementService.Application.V1.FetchEventById.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.V1.FetchEventById;

public static class FetchEventByIdServiceExtension
{
    public static IServiceCollection AddFetchEventById(this IServiceCollection collection)
    {
        collection.AddScoped<IEventRepository, EventRepository>();
        collection.AddScoped<IUserRepository, UserRepository>();

        return collection;
    }
}