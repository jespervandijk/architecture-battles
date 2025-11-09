using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Courses;

public class CreateCourseEndpoint : Endpoint<CreateCourse, CourseId>

{
    public override void Configure()
    {
        Post("academic-management/course/create");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateCourse req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record CreateCourse : ICommand<CourseId>
{
    public required UniversityId UniversityId { get; init; }
    public required DepartmentId DepartmentId { get; init; }
    public required ProfessorId CourseOwner { get; init; }
    public required Name Title { get; init; }
    public Description? Description { get; init; }
    public required Credits Credits { get; init; }
    public StudentCapacity? MaxCapacity { get; init; }
}

public class CreateCourseHandler : ICommandHandler<CreateCourse, CourseId>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<CourseId> ExecuteAsync(CreateCourse command, CancellationToken ct)
    {
        var course = Course.Create(command.UniversityId,
            command.DepartmentId,
            command.CourseOwner,
            command.Title,
            command.Credits,
            command.Description,
            command.MaxCapacity);
        _courseRepository.Insert(course);
        await _unitOfWork.SaveChangesAsync();
        return course.Id;
    }
}

public class CreateCourseValidator : Validator<CreateCourse>
{
    public CreateCourseValidator()
    {
        _ = RuleFor(x => x.Title).NotEmpty();
        _ = RuleFor(x => x.Credits).Must(c => c.Value > 0).WithMessage("Credits must be greater than zero.");
    }
}
