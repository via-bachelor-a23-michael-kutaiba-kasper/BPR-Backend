using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Infrastructure;

public static class InfrastructureServiceExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services;
    }
}