using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Courses;

public class UpdateSectionByProfessorEndpoint : Endpoint<UpdateSectionByProfessor>
{
    public override void Configure()
    {
        Post("academic-management/course/section/update-by-teacher");
        Policies(PolicyAcademicManagement.ProfessorOnly);
    }

    public override async Task HandleAsync(UpdateSectionByProfessor req, CancellationToken ct)
    {
        await req.ExecuteAsync(ct);
    }
}

public record UpdateSectionByProfessor : ICommand
{
    public required SectionId SectionId { get; init; }
    public required CourseId CourseId { get; init; }
    public required string Name { get; init; }
    public Uri? TeachingMaterialsUrl { get; init; }
}

public class UpdateSectionByProfessorHandler : ICommandHandler<UpdateSectionByProfessor>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateSectionByProfessorHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task ExecuteAsync(UpdateSectionByProfessor command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);
        var section = course.Sections.FirstOrDefault(s => s.Id == command.SectionId);
        if (section is null)
        {
            throw new InvalidOperationException("Section not found in this course");
        }

        var professorId = _userContextService.GetProfessorId();
        if (section.Professor != professorId)
        {
            throw new UnauthorizedAccessException("You must be the professor of this section");
        }

        course.UpdateSectionDetails(command.SectionId, command.Name, command.TeachingMaterialsUrl);

        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
    }
}
