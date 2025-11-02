using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Courses;
using FastEndpoints;
using Qowaiv;
using CourseId = AcademicManagement.Domain.Aggregates.Courses.CourseId;
using Credits = AcademicManagement.Domain.Aggregates.Courses.Credits;

namespace AcademicManagement.Application.UseCases.Courses;

public class CreateCourseEndpoint : Endpoint<CreateCourse, CourseId>

{
    public override void Configure()
    {
        Post("api/academic-management/course/create");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateCourse req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync();
    }
}

public record CreateCourse : ICommand<CourseId>
{
    public required Year Year { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required Credits Credits { get; init; }

    public required int MaxCapacity { get; init; }

    public required CourseStatus Status { get; init; }
}

public class CreateCourseValidator : Validator<CreateCourse>
{
    public CreateCourseValidator()
    {

    }
}

public class CreateCourseHandler : ICommandHandler<CreateCourse, CourseId>
{
    private readonly ICourseRepository _courseRepository;

    public CreateCourseHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    public async Task<CourseId> ExecuteAsync(CreateCourse command, CancellationToken ct)
    {
        var course = Course.Create(
            command.Title,
            command.Description,
            command.Credits,
            command.MaxCapacity,
            command.Status);
        await _courseRepository.Create(course);
        return course.Id;
    }
}
