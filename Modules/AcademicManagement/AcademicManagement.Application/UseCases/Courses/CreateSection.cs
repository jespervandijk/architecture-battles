using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;
using FluentValidation;

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
    private readonly IProfessorRepository _professorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CreateSectionHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IProfessorRepository professorRepository, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _professorRepository = professorRepository;
        _userContextService = userContextService;
    }

    public async Task<SectionId> ExecuteAsync(CreateSection command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);
        var professor = await _professorRepository.GetByIdAsync(command.ProfessorId);

        var currentProfessorId = _userContextService.GetProfessorId();
        if (course.CourseOwner != currentProfessorId)
        {
            throw new UnauthorizedAccessException("You must be the course owner");
        }

        if (professor.WorkPlace != course.University)
        {
            throw new InvalidOperationException("Professor must work at the same university as the course");
        }

        if (professor.DepartmentId != course.Department && professor.DepartmentId is not null)
        {
            throw new InvalidOperationException("Professor must be in the same department as the course or not assigned to any department");
        }

        course.CreateSection(command.Name, command.ProfessorId);

        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();

        return course.Sections.Last().Id;
    }
}

public class CreateSectionValidator : Validator<CreateSection>
{
    public CreateSectionValidator()
    {
        _ = RuleFor(x => x.CourseId).NotEmpty();
        _ = RuleFor(x => x.Name).NotEmpty();
        _ = RuleFor(x => x.ProfessorId).NotEmpty();
    }
}
