using System.Reflection;
using AcademicManagement.Application;
using AcademicManagement.Infrastructure;
using StudentEnrollment.Application;
using StudentEnrollment.Domain;
using StudentEnrollment.Infrastructure;
using Vogen;

namespace StartUp;

public static class ProjectRegistry
{
    public static IEnumerable<Type> GetValueObjects()
    {
        List<Assembly> assemblies =
        [
            typeof(DependencyInjectionDomain).Assembly,
            typeof(StudentEnrollment.Application.DependencyInjectionApplication).Assembly,
            typeof(AcademicManagement.Application.DependencyInjectionApplication).Assembly,
            typeof(InfrastructureDependencyInjection).Assembly
        ];
        return assemblies.SelectMany(assembly => assembly.GetTypes()).Where(type => type.HasGenericAttribute<ValueObjectAttribute<object>>());
    }

    public static IServiceCollection AddServicesAllModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFromAcademicManagementApplication();
        services.AddFromAcademicManagementInfrastructure(configuration);
        services.AddFromStudentEnrollmentApplication();
        services.AddFromStudentEnrollmentInfrastructure();
        return services;
    }

    public static List<Assembly> ApplicationLayers =>
        [
            typeof(StudentEnrollment.Application.DependencyInjectionApplication).Assembly,
            typeof(AcademicManagement.Application.DependencyInjectionApplication).Assembly
        ];

    private static bool HasGenericAttribute<TAttribute>(this Type type)
    {
        return type.GetCustomAttributes()
            .Any(attr => attr.GetType().IsGenericType &&
                        attr.GetType().GetGenericTypeDefinition() == typeof(TAttribute).GetGenericTypeDefinition());
    }
}
