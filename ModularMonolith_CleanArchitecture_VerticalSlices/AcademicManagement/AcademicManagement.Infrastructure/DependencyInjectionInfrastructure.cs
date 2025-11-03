using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Infrastructure.Repositories;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AcademicManagement.Infrastructure;

public static class InfrastructureDependencyInjection
{

    public static IServiceCollection AddFromAcademicManagementInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMarten(options =>
        {
            options.Connection(configuration.GetConnectionString("AcademicManagementDatabase"));
        }).UseLightweightSessions();

        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<ICourseRepository, CourseRepository>();
        services.AddTransient<IPresidentRepository, PresidentRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUniversityRepository, UniversityRepository>();
        services.AddTransient<IProfessorRepository, ProfessorRepository>();

        return services;
    }

}
