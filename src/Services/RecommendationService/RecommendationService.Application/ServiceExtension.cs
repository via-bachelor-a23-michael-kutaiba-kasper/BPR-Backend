using Microsoft.Extensions.DependencyInjection;
using RecommendationService.Application.V1.GetInterestSurvey;
using RecommendationService.Application.V1.GetRecommendations;
using RecommendationService.Application.V1.StoreInterestSurveyResult;

namespace RecommendationService.Application;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection collection)
    {
        collection.AddGetRecommendations();
        collection.AddStoreInterestSurveyResult();
        collection.AddGetInterestSurvey();
        
        return collection;
    }
}