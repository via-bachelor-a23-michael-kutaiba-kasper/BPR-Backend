using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Application.V1;
using UserManagementService.Application.V1.ProcessExpProgress;

namespace UserManagementService.Application;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddV1ServiceCollection();
        services.AddProcessExpProgress();
        return services;
    }
}
