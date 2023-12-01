using EventManagementService.Application.V1.FetchCategories.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.V1.FetchCategories;

public static class FetchCategoriesExtension
{
    public static IServiceCollection AddFetchCategories(this IServiceCollection services)
    {
        services.AddScoped<ISqlFetchCategories, SqlFetchCategories>();
        return services;
    }
}