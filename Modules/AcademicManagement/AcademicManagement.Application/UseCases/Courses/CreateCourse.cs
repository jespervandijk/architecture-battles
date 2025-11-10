using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Courses;

public class CreateCourseEndpoint : Endpoint<CreateCourse, CourseId>

{
    public override void Configure()
    {
        Post("academic-management/course/create");
        Policies(PolicyAcademicManagement.ProfessorOnly);
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
    private readonly IUniversityRepository _universityRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CreateCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IUniversityRepository universityRepository, IDepartmentRepository departmentRepository, IProfessorRepository professorRepository, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _universityRepository = universityRepository;
        _departmentRepository = departmentRepository;
        _professorRepository = professorRepository;
        _userContextService = userContextService;
    }
    public async Task<CourseId> ExecuteAsync(CreateCourse command, CancellationToken ct)
    {
        _ = await _universityRepository.GetByIdAsync(command.UniversityId);
        var department = await _departmentRepository.GetByIdAsync(command.DepartmentId);
        var courseOwnerProfessor = await _professorRepository.GetByIdAsync(command.CourseOwner);

        if (department.UniversityId != command.UniversityId)
        {
            throw new InvalidOperationException("Department must belong to the specified university");
        }

        var currentProfessorId = _userContextService.GetProfessorId();
        if (department.HeadOfDepartment != currentProfessorId)
        {
            throw new UnauthorizedAccessException("Only the head of department can create courses");
        }

        if (courseOwnerProfessor.WorkPlace != command.UniversityId)
        {
            throw new InvalidOperationException("Course owner must work at the specified university");
        }

        if (courseOwnerProfessor.DepartmentId != command.DepartmentId)
        {
            throw new InvalidOperationException("Course owner must be assigned to the specified department");
        }

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
