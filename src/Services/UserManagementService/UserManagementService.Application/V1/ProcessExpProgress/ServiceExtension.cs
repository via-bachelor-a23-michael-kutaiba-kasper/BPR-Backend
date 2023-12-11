using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;

namespace UserManagementService.Application.V1.ProcessExpProgress;

public static class ServiceExtension
{
    public static IServiceCollection AddProcessExpProgress(this IServiceCollection collection)
    {
        collection.AddScoped<IExpStrategy, NewAttendeesStrategy>();
        collection.AddScoped<IExpStrategy, NewlyCreatedEventsStrategy>();
        collection.AddScoped<IExpStrategy, NewReviewsStrategy>();
        collection.AddScoped<IExpStrategy, SurveyCompletedStrategy>();
        collection.AddScoped<IExpStrategy, NewAchievementStrategy>();

        collection.AddScoped<IAttendeesRepository, AttendeesRepository>();
        collection.AddScoped<IEventsRepository, EventsRepository>();
        collection.AddScoped<IInterestSurveyRepository, InterestSurveyRepository>();
        collection.AddScoped<IProgressRepository, ProgressRepository>();
        collection.AddScoped<IReviewRepository, ReviewRepository>();
        collection.AddScoped<IAchievementsRepository, AchievementsRepository>();

        return collection;
    }
}