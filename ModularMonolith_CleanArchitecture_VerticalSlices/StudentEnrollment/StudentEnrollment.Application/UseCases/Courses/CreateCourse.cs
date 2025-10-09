using FastEndpoints;
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

public class CreateCourse : ICommand<CourseId>
{

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
