using Microsoft.Extensions.DependencyInjection;

namespace AcademicManagement.Application;

public static class DependencyInjectionApplication
{
    public static IServiceCollection AddFromAcademicManagementApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserContextService, UserContextService>();
        return services;
    }
}
