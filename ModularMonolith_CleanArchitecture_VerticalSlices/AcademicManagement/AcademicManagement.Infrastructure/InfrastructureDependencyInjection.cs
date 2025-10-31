using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Infrastructure.Repositories;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AcademicManagement.Infrastructure;

public static class InfrastructureDependencyInjection
{

    public static IServiceCollection AddFromInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMarten(options =>
        {
            options.Connection(configuration.GetConnectionString("AcademicManagementDatabase"));
        }).UseLightweightSessions();

        services.AddTransient<ICourseRepository, CourseRepository>();

        return services;
    }

}
