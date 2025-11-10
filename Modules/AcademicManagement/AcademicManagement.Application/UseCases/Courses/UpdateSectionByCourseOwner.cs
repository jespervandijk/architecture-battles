using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace AcademicManagement.Application.UseCases.Courses;

public class UpdateSectionByCourseOwnerEndpoint : Endpoint<UpdateSectionByCourseOwner>
{
    public override void Configure()
    {
        Post("academic-management/course/section/update-by-course-owner");
        Policies(PolicyAcademicManagement.ProfessorOnly);
        Description(x => x.WithTags("academic-management/courses/section"));
    }

    public override async Task HandleAsync(UpdateSectionByCourseOwner req, CancellationToken ct)
    {
        await req.ExecuteAsync(ct);
    }
}

public record UpdateSectionByCourseOwner : ICommand
{
    public required SectionId SectionId { get; init; }
    public required CourseId CourseId { get; init; }
    public required string Name { get; init; }
    public required ProfessorId ProfessorId { get; init; }
}

public class UpdateSectionByCourseOwnerHandler : ICommandHandler<UpdateSectionByCourseOwner>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateSectionByCourseOwnerHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IProfessorRepository professorRepository, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _professorRepository = professorRepository;
        _userContextService = userContextService;
    }

    public async Task ExecuteAsync(UpdateSectionByCourseOwner command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);
        var professor = await _professorRepository.GetByIdAsync(command.ProfessorId);

        var currentProfessorId = _userContextService.GetProfessorId();
        if (course.CourseOwner != currentProfessorId)
        {
            throw new UnauthorizedAccessException("You must be the course owner");
        }

        var section = course.Sections.FirstOrDefault(s => s.Id == command.SectionId);
        if (section is null)
        {
            throw new InvalidOperationException("Section not found in this course");
        }

        if (professor.WorkPlace != course.University)
        {
            throw new InvalidOperationException("Professor must work at the same university as the course");
        }

        if (professor.DepartmentId != course.Department && professor.DepartmentId is not null)
        {
            throw new InvalidOperationException("Professor must be in the same department as the course or not assigned to any department");
        }

        course.UpdateSection(command.SectionId, command.Name, command.ProfessorId);

        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
    }
}
