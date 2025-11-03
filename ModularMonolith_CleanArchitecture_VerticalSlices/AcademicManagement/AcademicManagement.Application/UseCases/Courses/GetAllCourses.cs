using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Courses;

public class getAllCoursesEndpoint : EndpointWithoutRequest<IReadOnlyList<Course>>
{
    private readonly ICourseRepository _courseRepository;

    public getAllCoursesEndpoint(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public override void Configure()
    {
        Get("/courses");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Response = await _courseRepository.GetAllAsync();
    }
}
