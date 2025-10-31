using System.Reflection;
using AcademicManagement.Application;
using AcademicManagement.Infrastructure;
using StudentEnrollment.Application;
using StudentEnrollment.Domain;
using Vogen;

namespace StartUp;

public static class ProjectRegistry
{
    public static IEnumerable<Type> GetValueObjects()
    {
        List<Assembly> assemblies =
        [
            typeof(DependencyInjectionDomain).Assembly,
            typeof(DependencyInjectionApplication).Assembly,
            typeof(ApplicationDependencyInjection).Assembly,
            typeof(InfrastructureDependencyInjection).Assembly
        ];
        return assemblies.SelectMany(assembly => assembly.GetTypes()).Where(type => type.HasGenericAttribute<ValueObjectAttribute<object>>());
    }

    public static List<Assembly> ApplicationLayers =>
        [
            typeof(DependencyInjectionApplication).Assembly,
            typeof(ApplicationDependencyInjection).Assembly
        ];

    private static bool HasGenericAttribute<TAttribute>(this Type type)
    {
        return type.GetCustomAttributes()
            .Any(attr => attr.GetType().IsGenericType &&
                        attr.GetType().GetGenericTypeDefinition() == typeof(TAttribute).GetGenericTypeDefinition());
    }
}
