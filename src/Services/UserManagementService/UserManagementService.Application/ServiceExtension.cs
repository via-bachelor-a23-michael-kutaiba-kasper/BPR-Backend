using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Application.V1;

namespace UserManagementService.Application;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddV1ServiceCollection();
        return services;
    }
}
