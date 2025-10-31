using Microsoft.Extensions.DependencyInjection;

namespace AcademicManagement.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddFromApplication(this IServiceCollection services)
    {
        return services;
    }
}
