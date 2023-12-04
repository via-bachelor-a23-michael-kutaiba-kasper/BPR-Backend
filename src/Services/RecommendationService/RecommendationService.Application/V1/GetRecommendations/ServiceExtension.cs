using Microsoft.Extensions.DependencyInjection;

namespace RecommendationService.Application.V1.GetRecommendations;

internal static class ServiceExtension
{
    internal static IServiceCollection AddGetRecommendations(this IServiceCollection collection)
    {
        return collection;
    }
}