using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Assignments;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;
using Qowaiv;

namespace AcademicManagement.Application.UseCases.Assignments;

public class CreateAssignmentEndpoint : Endpoint<CreateAssignment, AssignmentId>
{
    public override void Configure()
    {
        Post("/assignments");
        Policies(PolicyAcademicManagement.ProfessorOnly);
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
    private readonly IUnitOfWork _unitOfWork;

    public CreateAssignmentHandler(IAssignmentRepository assignmentRepository, IUnitOfWork unitOfWork)
    {
        _assignmentRepository = assignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AssignmentId> ExecuteAsync(CreateAssignment command, CancellationToken ct)
    {
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

public class CreateAssignmentValidator : Validator<CreateAssignment>
{
    public CreateAssignmentValidator()
    {
        _ = RuleFor(x => x.SectionId).NotEmpty();
        _ = RuleFor(x => x.CourseId).NotEmpty();
        _ = RuleFor(x => x.Title).NotEmpty();
        _ = RuleFor(x => x.GradeWeight).NotEmpty();
        _ = RuleFor(x => x.SchoolYear).NotEmpty();
        _ = RuleFor(x => x.DocumentUrl).NotEmpty();

        _ = RuleFor(x => x.CourseId)
            .MustAsync(async (courseId, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var course = await courseRepo.GetByIdAsync(courseId);
                return course is not null;
            })
            .WithMessage("Course not found");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();

                var course = await courseRepo.GetByIdAsync(request.CourseId);
                return course?.Sections.Any(s => s.Id == request.SectionId) == true;
            })
            .WithMessage("Section not found in this course")
            .MustAsync(async (request, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var userContext = Resolve<IUserContextService>();

                var course = await courseRepo.GetByIdAsync(request.CourseId);
                var section = course?.Sections.FirstOrDefault(s => s.Id == request.SectionId);
                var currentUser = userContext.GetCurrentUser();
                var professorId = ProfessorId.From(currentUser.Id.Value);

                return section?.Professor == professorId;
            })
            .WithMessage("Only the section professor can create assignments");
    }
}
