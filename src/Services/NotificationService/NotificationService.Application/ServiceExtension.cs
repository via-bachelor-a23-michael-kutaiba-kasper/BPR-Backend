using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Application;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }
}
