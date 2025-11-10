using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Application.Validation;
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
    private readonly IUnitOfWork _unitOfWork;

    public CreateExamHandler(IExamRepository examRepository, IUnitOfWork unitOfWork)
    {
        _examRepository = examRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ExamId> ExecuteAsync(CreateExam command, CancellationToken ct)
    {
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
        _ = RuleFor(x => x.SectionId).NotEmpty();
        _ = RuleFor(x => x.CourseId).NotEmpty();
        _ = RuleFor(x => x.Title).NotEmpty();
        _ = RuleFor(x => x.Duration).GreaterThan(TimeSpan.Zero);
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
                return await AuthorizationRules.UserIsSectionProfessor(
                    Resolve<IUserContextService>(),
                    Resolve<ICourseRepository>(),
                    request.CourseId,
                    request.SectionId);
            })
            .WithMessage("You must be the professor of this section");
    }
}
