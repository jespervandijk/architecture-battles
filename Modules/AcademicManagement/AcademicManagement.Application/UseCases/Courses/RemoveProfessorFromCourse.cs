using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace AcademicManagement.Application.UseCases.Courses;

public class RemoveProfessorFromCourseEndpoint : Endpoint<RemoveProfessorFromCourse, CourseId>
{
    public override void Configure()
    {
        Post("academic-management/courses/remove-professor");
        Policies(PolicyAcademicManagement.ProfessorOnly);
        Description(x => x.WithTags("academic-management/courses"));
    }

    public override async Task HandleAsync(RemoveProfessorFromCourse req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record RemoveProfessorFromCourse : ICommand<CourseId>
{
    public required CourseId CourseId { get; init; }
    public required ProfessorId ProfessorId { get; init; }
}

public class RemoveProfessorFromCourseHandler : ICommandHandler<RemoveProfessorFromCourse, CourseId>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public RemoveProfessorFromCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository, IProfessorRepository professorRepository, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _departmentRepository = departmentRepository;
        _professorRepository = professorRepository;
        _userContextService = userContextService;
    }

    public async Task<CourseId> ExecuteAsync(RemoveProfessorFromCourse command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);
        _ = await _professorRepository.GetByIdAsync(command.ProfessorId);
        var department = await _departmentRepository.GetByIdAsync(course.Department);

        var currentProfessorId = _userContextService.GetProfessorId();
        if (department.HeadOfDepartment != currentProfessorId)
        {
            throw new UnauthorizedAccessException("Only the head of department can remove professors from courses");
        }

        course.RemoveProfessor(command.ProfessorId);
        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
        return course.Id;
    }
}
