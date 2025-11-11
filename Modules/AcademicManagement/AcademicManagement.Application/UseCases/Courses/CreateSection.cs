using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Courses;

public class CreateSectionEndpoint : Endpoint<CreateSection, SectionId>
{
    public override void Configure()
    {
        Post("academic-management/course/section/create");
        Policies(PolicyAcademicManagement.ProfessorOnly);
    }

    public override async Task HandleAsync(CreateSection req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record CreateSection : ICommand<SectionId>
{
    public required CourseId CourseId { get; init; }
    public required string Name { get; init; }
    public required ProfessorId ProfessorId { get; init; }
}

public class CreateSectionHandler : ICommandHandler<CreateSection, SectionId>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CreateSectionHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<SectionId> ExecuteAsync(CreateSection command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);

        var currentProfessorId = _userContextService.GetProfessorId();
        if (course.CourseOwner != currentProfessorId)
        {
            throw new UnauthorizedAccessException("You must be the course owner");
        }

        var newSectionId = course.CreateSection(command.Name, command.ProfessorId);
        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
        return newSectionId;
    }
}
