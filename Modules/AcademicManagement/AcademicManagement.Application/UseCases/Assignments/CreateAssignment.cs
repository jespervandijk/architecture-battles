using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Application.Exceptions;
using AcademicManagement.Domain.Aggregates.Assignments;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Qowaiv;

namespace AcademicManagement.Application.UseCases.Assignments;

public class CreateAssignmentEndpoint : Endpoint<CreateAssignment, AssignmentId>
{
    public override void Configure()
    {
        Post("academic-management/assignments/create");
        Policies(PolicyAcademicManagement.ProfessorOnly);
        Description(x => x.WithTags("academic-management/assignments"));
    }

    public override async Task HandleAsync(CreateAssignment req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record CreateAssignment : ICommand<AssignmentId>
{
    public required SectionId SectionId { get; init; }
    public required CourseId CourseId { get; init; }
    public required Name Title { get; init; }
    public required GradeWeight GradeWeight { get; init; }
    public required Year SchoolYear { get; init; }
    public required Url DocumentUrl { get; init; }
}

public class CreateAssignmentHandler : ICommandHandler<CreateAssignment, AssignmentId>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CreateAssignmentHandler(IAssignmentRepository assignmentRepository, IUnitOfWork unitOfWork, ICourseRepository courseRepository, IUserContextService userContextService)
    {
        _assignmentRepository = assignmentRepository;
        _unitOfWork = unitOfWork;
        _courseRepository = courseRepository;
        _userContextService = userContextService;
    }

    public async Task<AssignmentId> ExecuteAsync(CreateAssignment command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);
        var section = (course?.Sections.FirstOrDefault(s => s.Id == command.SectionId)) ?? throw new EntityNotFoundException(nameof(Section), command.SectionId);

        var professorId = _userContextService.GetProfessorId();
        if (section.Professor != professorId)
        {
            throw new UnauthorizedAccessException("You are not the professor of this section.");
        }

        var assignment = Assignment.Create(
            command.SectionId,
            command.Title,
            command.GradeWeight,
            command.SchoolYear,
            command.DocumentUrl
        );

        _assignmentRepository.Insert(assignment);
        await _unitOfWork.SaveChangesAsync();

        return assignment.Id;
    }
}
