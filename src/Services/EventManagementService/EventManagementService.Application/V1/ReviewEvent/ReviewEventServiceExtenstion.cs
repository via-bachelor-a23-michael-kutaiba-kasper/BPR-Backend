using EventManagementService.Application.V1.ReviewEvent.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.V1.ReviewEvent;

public static class ReviewEventServiceExtenstion
{
    public static IServiceCollection AddReviewEventServices(this IServiceCollection services)
    {
        services.AddScoped<ISqlReviewEvent, SqlReviewEvent>();
        services.AddScoped<IPubSubReviewEvent, PubSubReviewEvent>();
        return services;
    }
}