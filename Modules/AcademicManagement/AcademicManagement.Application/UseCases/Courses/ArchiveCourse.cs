using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using FastEndpoints;

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
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public ArchiveCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _departmentRepository = departmentRepository;
        _userContextService = userContextService;
    }

    public async Task ExecuteAsync(ArchiveCourse command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);
        var department = await _departmentRepository.GetByIdAsync(course.Department);

        var professorId = _userContextService.GetProfessorId();
        if (department.HeadOfDepartment != professorId)
        {
            throw new UnauthorizedAccessException("You must be the head of the department that owns this course.");
        }

        course.Archive();
        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
    }
}
