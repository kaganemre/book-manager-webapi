using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace BookManager.Application;
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddFastEndpoints();

        return services;
    }
}