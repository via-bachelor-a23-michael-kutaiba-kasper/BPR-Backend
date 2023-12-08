using Microsoft.Extensions.DependencyInjection;

namespace UserManagementService.Application.V1.ProcessExpProgress;

public static class ServiceExtension
{
    public static IServiceCollection AddProcessExpProgress(this IServiceCollection collection)
    {
        return collection;
    }
}