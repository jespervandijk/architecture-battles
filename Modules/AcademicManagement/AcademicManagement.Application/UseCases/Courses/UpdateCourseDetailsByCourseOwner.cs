using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Application.Validation;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Courses;

public class UpdateCourseDetailsByCourseOwnerEndpoint : Endpoint<UpdateCourseDetailsByCourseOwner, CourseId>
{
    public override void Configure()
    {
        Post("academic-management/course/update-details");
        Policies(PolicyAcademicManagement.ProfessorOnly);
    }

    public override async Task HandleAsync(UpdateCourseDetailsByCourseOwner req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record UpdateCourseDetailsByCourseOwner : ICommand<CourseId>
{
    public required CourseId CourseId { get; init; }
    public required Name Title { get; init; }
    public Description? Description { get; init; }
    public StudentCapacity? MaxCapacity { get; init; }
}

public class UpdateCourseDetailsByCourseOwnerHandler : ICommandHandler<UpdateCourseDetailsByCourseOwner, CourseId>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseDetailsByCourseOwnerHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CourseId> ExecuteAsync(UpdateCourseDetailsByCourseOwner command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId) ?? throw new InvalidOperationException($"Course with id {command.CourseId} was not found.");
        course.UpdateCourseDetails(command.Title, command.Description, command.MaxCapacity);
        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
        return course.Id;
    }
}

public class UpdateCourseDetailsByCourseOwnerValidator : Validator<UpdateCourseDetailsByCourseOwner>
{
    public UpdateCourseDetailsByCourseOwnerValidator()
    {
        _ = RuleFor(x => x.CourseId).NotEmpty();
        _ = RuleFor(x => x.Title).NotEmpty();

        _ = RuleFor(x => x.CourseId)
            .MustAsync(async (courseId, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var course = await courseRepo.GetByIdAsync(courseId);
                return course is not null;
            })
            .WithMessage("Course not found")
            .MustAsync(async (courseId, ct) =>
            {
                return await AuthorizationRules.UserIsCourseOwner(
                    Resolve<IUserContextService>(),
                    Resolve<ICourseRepository>(),
                    courseId);
            })
            .WithMessage("You must be the course owner");
    }
}
