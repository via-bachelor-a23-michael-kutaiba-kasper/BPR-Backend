using Microsoft.Extensions.DependencyInjection;
using FirebaseInterestSurveyRepository = RecommendationService.Application.V1.GetInterestSurvey.Repositories.FirebaseInterestSurveyRepository;
using FirebaseUserRepository = RecommendationService.Application.V1.GetInterestSurvey.Repositories.FirebaseUserRepository;
using IInterestSurveyRepository = RecommendationService.Application.V1.GetInterestSurvey.Repositories.IInterestSurveyRepository;
using IUserRepository = RecommendationService.Application.V1.GetInterestSurvey.Repositories.IUserRepository;

namespace RecommendationService.Application.V1.GetInterestSurvey;

public static class ServiceExtension
{
    public static IServiceCollection AddGetInterestSurvey(this IServiceCollection collection)
    {
        collection.AddScoped<IInterestSurveyRepository, FirebaseInterestSurveyRepository>();
        collection.AddScoped<IUserRepository, FirebaseUserRepository>();
        
        return collection;
    }
}