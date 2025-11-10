using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Courses;

public class UpdateSectionByProfessorEndpoint : Endpoint<UpdateSectionByProfessor>
{
    public override void Configure()
    {
        Post("academic-management/course/section/update-by-teacher");
        Policies(PolicyAcademicManagement.ProfessorOnly);
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

    public UpdateSectionByProfessorHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(UpdateSectionByProfessor command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId) ?? throw new InvalidOperationException($"Course with id {command.CourseId} not found.");

        course.UpdateSectionDetails(command.SectionId, command.Name, command.TeachingMaterialsUrl);

        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
    }
}

public class UpdateSectionByProfessorValidator : Validator<UpdateSectionByProfessor>
{
    public UpdateSectionByProfessorValidator()
    {
        _ = RuleFor(x => x.SectionId).NotEmpty();
        _ = RuleFor(x => x.CourseId).NotEmpty();
        _ = RuleFor(x => x.Name).NotEmpty();

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
            .WithMessage("Only the section professor can use this endpoint.");
    }
}
