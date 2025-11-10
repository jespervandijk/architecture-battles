using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Courses;

public class AssignProfessorToCourseEndpoint : Endpoint<AssignProfessorToCourse, CourseId>
{
    public override void Configure()
    {
        Post("academic-management/course/assign-professor");
        Policies(PolicyAcademicManagement.ProfessorOnly);
    }

    public override async Task HandleAsync(AssignProfessorToCourse req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record AssignProfessorToCourse : ICommand<CourseId>
{
    public required CourseId CourseId { get; init; }
    public required ProfessorId ProfessorId { get; init; }
    public bool AsCourseOwner { get; init; }
}

public class AssignProfessorToCourseHandler : ICommandHandler<AssignProfessorToCourse, CourseId>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignProfessorToCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CourseId> ExecuteAsync(AssignProfessorToCourse command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId) ?? throw new InvalidOperationException($"Course with id {command.CourseId} was not found.");

        if (command.AsCourseOwner)
        {
            course.AssignProfessorAsCourseOwner(command.ProfessorId);
        }
        else
        {
            course.AssignProfessor(command.ProfessorId);
        }

        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
        return course.Id;
    }
}

public class AssignProfessorToCourseValidator : Validator<AssignProfessorToCourse>
{
    public AssignProfessorToCourseValidator()
    {
        _ = RuleFor(x => x.CourseId).NotEmpty();
        _ = RuleFor(x => x.ProfessorId).NotEmpty();

        _ = RuleFor(x => x.CourseId)
            .MustAsync(async (courseId, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var course = await courseRepo.GetByIdAsync(courseId);
                return course is not null;
            })
            .WithMessage("Course not found");

        _ = RuleFor(x => x.ProfessorId)
            .MustAsync(async (professorId, ct) =>
            {
                var professorRepo = Resolve<IProfessorRepository>();
                var professor = await professorRepo.GetByIdAsync(professorId);
                return professor is not null;
            })
            .WithMessage("Professor not found");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var departmentRepo = Resolve<IDepartmentRepository>();
                var userContext = Resolve<IUserContextService>();

                var course = await courseRepo.GetByIdAsync(request.CourseId);
                var department = await departmentRepo.GetByIdAsync(course.Department);
                var currentUser = userContext.GetCurrentUser();

                var currentProfessorId = ProfessorId.From(currentUser.Id.Value);
                return department.HeadOfDepartment == currentProfessorId;
            })
            .WithMessage("Only the head of department can assign professors to courses");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var professorRepo = Resolve<IProfessorRepository>();

                var course = await courseRepo.GetByIdAsync(request.CourseId);
                var professor = await professorRepo.GetByIdAsync(request.ProfessorId);

                return professor.DepartmentId == course.Department;
            })
            .WithMessage("Professor must be assigned to the course's department");
    }
}
