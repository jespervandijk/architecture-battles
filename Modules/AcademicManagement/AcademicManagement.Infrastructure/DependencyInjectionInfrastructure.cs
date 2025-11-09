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
        _ = services.AddMarten(options =>
        {
            options.Connection(configuration.GetConnectionString("Marten")!);
            options.UseSystemTextJsonForSerialization(enumStorage: Weasel.Core.EnumStorage.AsString);
        }).UseLightweightSessions();

        return services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<ICourseRepository, CourseRepository>()
            .AddScoped<IPresidentRepository, PresidentRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUniversityRepository, UniversityRepository>()
            .AddScoped<IProfessorRepository, ProfessorRepository>();
    }

}
