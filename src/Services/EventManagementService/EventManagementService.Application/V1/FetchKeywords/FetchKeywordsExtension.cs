using EventManagementService.Application.V1.FetchKeywords.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.V1.FetchKeywords;

public static class FetchKeywordsExtension
{
    public static IServiceCollection AddFetchKeywords(this IServiceCollection services)
    {
        services.AddScoped<ISqlFetchKeywords, SqlFetchKeywords>();
        return services;
    }
}