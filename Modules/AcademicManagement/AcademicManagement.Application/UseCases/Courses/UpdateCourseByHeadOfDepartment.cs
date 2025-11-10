using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;

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
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateCourseByHeadOfDepartmentHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository, IProfessorRepository professorRepository, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _departmentRepository = departmentRepository;
        _professorRepository = professorRepository;
        _userContextService = userContextService;
    }

    public async Task<CourseId> ExecuteAsync(UpdateCourseByHeadOfDepartment command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);
        var department = await _departmentRepository.GetByIdAsync(course.Department);
        var courseOwnerProfessor = await _professorRepository.GetByIdAsync(command.CourseOwner);

        var currentProfessorId = _userContextService.GetProfessorId();
        if (department.HeadOfDepartment != currentProfessorId)
        {
            throw new UnauthorizedAccessException("You must be the head of the department that owns this course");
        }

        if (courseOwnerProfessor.DepartmentId != course.Department)
        {
            throw new InvalidOperationException("Course owner must be assigned to the course's department");
        }

        course.UpdateCourse(command.CourseOwner, command.Title, command.Description, command.Credits, command.MaxCapacity);
        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
        return course.Id;
    }
}
