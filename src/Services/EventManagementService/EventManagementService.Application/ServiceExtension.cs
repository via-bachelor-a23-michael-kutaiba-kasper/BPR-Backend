using EventManagementService.Application.V1;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Application;
public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddV1Services();
        return services;
    }
}
