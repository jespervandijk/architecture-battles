using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Exams;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;
using Qowaiv;

namespace AcademicManagement.Application.UseCases.Exams;

public class CreateExamEndpoint : Endpoint<CreateExam, ExamId>
{
    public override void Configure()
    {
        Post("/exams");
        Policies(PolicyAcademicManagement.ProfessorOnly);
    }

    public override async Task HandleAsync(CreateExam req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record CreateExam : ICommand<ExamId>
{
    public SectionId SectionId { get; init; }
    public CourseId CourseId { get; init; }
    public Name Title { get; init; }
    public TimeSpan Duration { get; init; }
    public GradeWeight GradeWeight { get; init; }
    public Year SchoolYear { get; init; }
    public Url DocumentUrl { get; init; }
}

public class CreateExamHandler : ICommandHandler<CreateExam, ExamId>
{
    private readonly IExamRepository _examRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CreateExamHandler(IExamRepository examRepository, IUnitOfWork unitOfWork, ICourseRepository courseRepository, IUserContextService userContextService)
    {
        _examRepository = examRepository;
        _unitOfWork = unitOfWork;
        _courseRepository = courseRepository;
        _userContextService = userContextService;
    }

    public async Task<ExamId> ExecuteAsync(CreateExam command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);
        var section = course.GetSection(command.SectionId);

        var professorId = _userContextService.GetProfessorId();
        if (section.Professor != professorId)
        {
            throw new UnauthorizedAccessException("You must be the professor of this section");
        }

        var exam = Exam.Create(
            command.SectionId,
            command.Title,
            command.Duration,
            command.GradeWeight,
            command.SchoolYear,
            command.DocumentUrl
        );

        _examRepository.Insert(exam);
        await _unitOfWork.SaveChangesAsync();

        return exam.Id;
    }
}

public class CreateExamValidator : Validator<CreateExam>
{
    public CreateExamValidator()
    {
        _ = RuleFor(x => x.Duration).GreaterThan(TimeSpan.Zero);
    }
}
