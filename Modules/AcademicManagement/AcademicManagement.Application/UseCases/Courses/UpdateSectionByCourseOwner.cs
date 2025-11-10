using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Courses;

public class UpdateSectionByCourseOwnerEndpoint : Endpoint<UpdateSectionByCourseOwner>
{
    public override void Configure()
    {
        Post("academic-management/course/section/update-by-course-owner");
        Policies(PolicyAcademicManagement.ProfessorOnly);
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
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSectionByCourseOwnerHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(UpdateSectionByCourseOwner command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId) ?? throw new InvalidOperationException($"Course with id {command.CourseId} not found.");

        course.UpdateSection(command.SectionId, command.Name, command.ProfessorId);

        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
    }
}

public class UpdateSectionByCourseOwnerValidator : Validator<UpdateSectionByCourseOwner>
{
    public UpdateSectionByCourseOwnerValidator()
    {
        _ = RuleFor(x => x.SectionId).NotEmpty();
        _ = RuleFor(x => x.CourseId).NotEmpty();
        _ = RuleFor(x => x.Name).NotEmpty();
        _ = RuleFor(x => x.ProfessorId).NotEmpty();

        _ = RuleFor(x => x.CourseId)
            .MustAsync(async (courseId, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var course = await courseRepo.GetByIdAsync(courseId);
                return course is not null;
            })
            .WithMessage("Course not found")
            .MustAsync(async (courseId, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var userContext = Resolve<IUserContextService>();

                var course = await courseRepo.GetByIdAsync(courseId);
                var currentUser = userContext.GetCurrentUser();
                var professorId = ProfessorId.From(currentUser.Id.Value);

                return course?.CourseOwner == professorId;
            })
            .WithMessage("Only the course owner can use this endpoint.");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();

                var course = await courseRepo.GetByIdAsync(request.CourseId);
                return course?.Sections.Any(s => s.Id == request.SectionId) == true;
            })
            .WithMessage("Section not found in this course");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var professorRepo = Resolve<IProfessorRepository>();

                var course = await courseRepo.GetByIdAsync(request.CourseId);
                var professor = await professorRepo.GetByIdAsync(request.ProfessorId);

                return professor?.WorkPlace == course?.University;
            })
            .WithMessage("Professor must work at the same university as the course");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                var courseRepo = Resolve<ICourseRepository>();
                var professorRepo = Resolve<IProfessorRepository>();

                var course = await courseRepo.GetByIdAsync(request.CourseId);
                var professor = await professorRepo.GetByIdAsync(request.ProfessorId);

                return professor?.DepartmentId == course?.Department || professor?.DepartmentId is null;
            })
            .WithMessage("Professor must be in the same department as the course or not assigned to any department");
    }
}
