using Microsoft.Extensions.DependencyInjection;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Repositories;

namespace RecommendationService.Application.V1.StoreInterestSurveyResult;

public static class ServiceExtension
{
    public static IServiceCollection AddStoreInterestSurveyResult(this IServiceCollection collection)
    {
        collection.AddScoped<IUserRepository, FirebaseUserRepository>();
        collection.AddScoped<IInterestSurveyRepository, FirebaseInterestSurveyRepository>();
        
        return collection;
    }
}