using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Application.Dtos;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Courses;

public class GetAllCoursesEndpoint : Endpoint<GetCourses, IReadOnlyList<CourseDto>>
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
        var courses = await _courseRepository.GetAllAsync();
        var filteredCourses = courses.Where(course =>
            (req.UniversityId == null || course.University == req.UniversityId) &&
            (req.DepartmentId == null || course.Department == req.DepartmentId) &&
            (req.CourseOwnerId == null || course.CourseOwner == req.CourseOwnerId) &&
            (req.ProfessorId == null || course.Professors.Contains(req.ProfessorId.Value)) &&
            (req.Title == null || course.Title.Value.Contains(req.Title.Value.Value, StringComparison.OrdinalIgnoreCase))
        );
        Response = [.. courses.Select(CourseDto.FromDomain)];
    }
}

public record GetCourses
{
    public UniversityId? UniversityId { get; init; }
    public DepartmentId? DepartmentId { get; init; }
    public ProfessorId? CourseOwnerId { get; init; }
    public ProfessorId? ProfessorId { get; init; }
    public Name? Title { get; init; }
}

public class GetCoursesValidator : Validator<GetCourses>
{
    public GetCoursesValidator()
    {
    }
}
