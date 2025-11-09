using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Courses;

public class GetAllCoursesEndpoint : EndpointWithoutRequest<IReadOnlyList<Course>>
{
    private readonly ICourseRepository _courseRepository;

    public GetAllCoursesEndpoint(ICourseRepository courseRepository)
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

public record GetCourses : ICommand<IReadOnlyList<Course>>
{

}
