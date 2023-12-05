using EventManagementService.Application.V1.FetchReviewsByUser.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.V1.FetchReviewsByUser;

public static class FetchReviewsByUserExtenstion
{
    public static IServiceCollection AddFetchReviewsByUser(this IServiceCollection services)
    {
        services.AddScoped<ISqlReviews, SqlReviews>();
        return services;
    }
}