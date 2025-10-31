using System;
using Microsoft.Extensions.DependencyInjection;

namespace StudentEnrollment.Infrastructure;

public static class DependencyInjectionInfrastructure
{
    public static IServiceCollection AddFromStudentEnrollmentInfrastructure(this IServiceCollection services)
    {
        return services;
    }
}
