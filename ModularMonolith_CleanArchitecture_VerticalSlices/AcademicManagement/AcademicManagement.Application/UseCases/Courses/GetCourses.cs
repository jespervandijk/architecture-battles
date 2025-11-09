using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Courses;

public class GetAllCoursesEndpoint : Endpoint<GetCourses, IReadOnlyList<Course>>
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

    public override async Task HandleAsync(GetCourses req, CancellationToken ct)
    {
        Response = await _courseRepository.GetAllAsync();
    }
}

public record GetCourses
{
    public UniversityId UniversityId { get; init; }
    public DepartmentId DepartmentId { get; init; }
    public ProfessorId CourseOwnerId { get; init; }
    public ProfessorId ProfessorId { get; init; }
    public Name Title { get; init; }
}

public class GetCoursesValidator : Validator<GetCourses>
{
    public GetCoursesValidator()
    {
    }
}
