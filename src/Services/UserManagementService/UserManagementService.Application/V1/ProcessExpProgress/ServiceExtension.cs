using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;

namespace UserManagementService.Application.V1.ProcessExpProgress;

public static class ServiceExtension
{
    public static IServiceCollection AddProcessExpProgress(this IServiceCollection collection)
    {
        collection.AddScoped<IExpStrategy, NewAttendeesStrategy>();
        collection.AddScoped<IExpStrategy, NewlyCreatedEventsStrategy>();
        collection.AddScoped<IExpStrategy, NewReviewsStrategy>();
        collection.AddScoped<IExpStrategy, SurveyCompletedStrategy>();
        
        return collection;
    }
}