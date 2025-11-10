using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Application.Validation;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Courses;

public class UpdateCourseByHeadOfDepartmentEndpoint : Endpoint<UpdateCourseByHeadOfDepartment, CourseId>
{
    public override void Configure()
    {
        Post("academic-management/course/update-by-head");
        Policies(PolicyAcademicManagement.ProfessorOnly);
    }

    public override async Task HandleAsync(UpdateCourseByHeadOfDepartment req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record UpdateCourseByHeadOfDepartment : ICommand<CourseId>
{
    public required CourseId CourseId { get; init; }
    public required ProfessorId CourseOwner { get; init; }
    public required Name Title { get; init; }
    public Description? Description { get; init; }
    public required Credits Credits { get; init; }
    public StudentCapacity? MaxCapacity { get; init; }
}

public class UpdateCourseByHeadOfDepartmentHandler : ICommandHandler<UpdateCourseByHeadOfDepartment, CourseId>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseByHeadOfDepartmentHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CourseId> ExecuteAsync(UpdateCourseByHeadOfDepartment command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId) ?? throw new InvalidOperationException($"Course with id {command.CourseId} was not found.");
        course.UpdateCourse(command.CourseOwner, command.Title, command.Description, command.Credits, command.MaxCapacity);
        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
        return course.Id;
    }
}

public class UpdateCourseByHeadOfDepartmentValidator : Validator<UpdateCourseByHeadOfDepartment>
{
    public UpdateCourseByHeadOfDepartmentValidator()
    {
        _ = RuleFor(x => x.CourseId).NotEmpty();
        _ = RuleFor(x => x.CourseOwner).NotEmpty();
        _ = RuleFor(x => x.Title).NotEmpty();
        _ = RuleFor(x => x.Credits).Must(c => c.Value > 0).WithMessage("Credits must be greater than zero.");

        _ = RuleFor(x => x.CourseId)
            .MustAsync(async (courseId, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var course = await courseRepo.GetByIdAsync(courseId);
                return course is not null;
            })
            .WithMessage("Course not found");

        _ = RuleFor(x => x.CourseOwner)
            .MustAsync(async (courseOwner, ct) =>
            {
                var professorRepo = Resolve<IProfessorRepository>();
                var professor = await professorRepo.GetByIdAsync(courseOwner);
                return professor is not null;
            })
            .WithMessage("Professor not found");

        _ = RuleFor(x => x.CourseId)
            .MustAsync(async (courseId, ct) =>
            {
                return await AuthorizationRules.UserIsHeadOfCourseDepartment(
                    Resolve<IUserContextService>(),
                    Resolve<ICourseRepository>(),
                    Resolve<IDepartmentRepository>(),
                    courseId);
            })
            .WithMessage("You must be the head of the department that owns this course");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var professorRepo = Resolve<IProfessorRepository>();

                var course = await courseRepo.GetByIdAsync(request.CourseId);
                var professor = await professorRepo.GetByIdAsync(request.CourseOwner);

                return professor.DepartmentId == course.Department;
            })
            .WithMessage("Course owner must be assigned to the course's department");
    }
}
