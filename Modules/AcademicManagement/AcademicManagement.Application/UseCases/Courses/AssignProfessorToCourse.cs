using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;

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
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public AssignProfessorToCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository, IProfessorRepository professorRepository, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _departmentRepository = departmentRepository;
        _professorRepository = professorRepository;
        _userContextService = userContextService;
    }

    public async Task<CourseId> ExecuteAsync(AssignProfessorToCourse command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);
        var department = await _departmentRepository.GetByIdAsync(course.Department);
        var professor = await _professorRepository.GetByIdAsync(command.ProfessorId);

        var currentProfessorId = _userContextService.GetProfessorId();
        if (department.HeadOfDepartment != currentProfessorId)
        {
            throw new UnauthorizedAccessException("Only the head of department can assign professors to courses.");
        }

        if (professor.DepartmentId != course.Department)
        {
            throw new InvalidOperationException("Professor must be assigned to the course's department.");
        }

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
