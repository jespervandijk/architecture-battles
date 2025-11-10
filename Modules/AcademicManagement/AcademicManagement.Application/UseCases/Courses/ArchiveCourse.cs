using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Courses;

public class ArchiveCourseEndpoint : Endpoint<ArchiveCourse>
{
    public override void Configure()
    {
        Post("academic-management/course/archive");
        Policies(PolicyAcademicManagement.ProfessorOnly);
    }

    public override async Task HandleAsync(ArchiveCourse req, CancellationToken ct)
    {
        await req.ExecuteAsync(ct);
    }
}

public record ArchiveCourse : ICommand
{
    public required CourseId CourseId { get; init; }
}

public class ArchiveCourseHandler : ICommandHandler<ArchiveCourse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ArchiveCourse command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId) ?? throw new InvalidOperationException("Course not found");
        course.Archive();
        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
    }
}

public class ArchiveCourseValidator : Validator<ArchiveCourse>
{
    public ArchiveCourseValidator()
    {
        _ = RuleFor(x => x.CourseId)
            .NotEmpty()
            .MustAsync(async (courseId, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var course = await courseRepo.GetByIdAsync(courseId);
                return course is not null;
            })
            .WithMessage("Course not found")
            .MustAsync(async (courseId, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var departmentRepo = Resolve<IDepartmentRepository>();
                var userContext = Resolve<IUserContextService>();

                var course = await courseRepo.GetByIdAsync(courseId);
                var department = await departmentRepo.GetByIdAsync(course.Department);
                var currentUser = userContext.GetCurrentUser();

                var professorId = ProfessorId.From(currentUser.Id.Value);
                return department.HeadOfDepartment == professorId;
            })
            .WithMessage("Only the head of department can archive courses");
    }
}
