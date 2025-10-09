using FastEndpoints;
using Qowaiv;
using StudentEnrollment.Domain.Courses;

namespace StudentEnrollment.Application.UseCases.Courses;

public class CreateCourseEndpoint : Endpoint<CreateCourse, CourseId>
{
    public override void Configure()
    {
        Post("api/course/create");
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

    public required Semester Semester { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }
}

public class CreateCourseValidator : Validator<CreateCourse>
{
    public CreateCourseValidator()
    {

    }
}

public class CreateCourseHandler : ICommandHandler<CreateCourse, CourseId>
{
    public Task<CourseId> ExecuteAsync(CreateCourse command, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
