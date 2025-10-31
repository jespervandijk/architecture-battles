using Microsoft.Extensions.DependencyInjection;

namespace StudentEnrollment.Application;

public static class DependencyInjectionApplication
{
    public static IServiceCollection AddFromStudentEnrollmentApplication(this IServiceCollection services)
    {
        return services;
    }

}
