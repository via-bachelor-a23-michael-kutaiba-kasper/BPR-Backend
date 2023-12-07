using Microsoft.Extensions.DependencyInjection;
using RecommendationService.Application.V1.GetRecommendations;

namespace RecommendationService.Application;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection collection)
    {
        collection.AddGetRecommendations();
        
        return collection;
    }
}