using EventManagementService.Application.FetchKeywords.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application.FetchKeywords;

public static class FetchKeywordsExtension
{
    public static IServiceCollection AddFetchKeywords(this IServiceCollection services)
    {
        services.AddScoped<ISqlFetchKeywords, SqlFetchKeywords>();
        return services;
    }
}