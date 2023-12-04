using Microsoft.Extensions.DependencyInjection;

namespace RecommendationService.Application;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection collection)
    {
        return collection;
    }
}