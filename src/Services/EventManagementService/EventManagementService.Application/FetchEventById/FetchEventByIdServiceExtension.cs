using EventManagementService.Application.FetchEventById.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.FetchEventById;

public static class FetchEventByIdServiceExtension
{
    public static IServiceCollection AddFetchEventById(this IServiceCollection collection)
    {
        collection.AddScoped<IEventRepository, EventRepository>();
        
        return collection;
    }
}