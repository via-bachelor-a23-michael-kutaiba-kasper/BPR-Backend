using Microsoft.Extensions.DependencyInjection;
using RecommendationService.Application.V1.GetRecommendations.Repository;

namespace RecommendationService.Application.V1.GetRecommendations;

internal static class ServiceExtension
{
    internal static IServiceCollection AddGetRecommendations(this IServiceCollection collection)
    {
        collection.AddScoped<IEventsRepository, EventsRepository>();
        collection.AddScoped<IReviewRepository, ReviewRepository>();
        collection.AddScoped<ISurveyRepository, SurveyRepository>();
        collection.AddScoped<IUserRepository, UserRepository>();
        
        return collection;
    }
}