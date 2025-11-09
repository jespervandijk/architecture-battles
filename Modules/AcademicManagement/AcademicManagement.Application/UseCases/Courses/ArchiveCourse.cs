using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Courses;

public class ArchiveCourseEndpoint : Endpoint<ArchiveCourse>
{
    public override void Configure()
    {
        Post("academic-management/course/archive");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ArchiveCourse req, CancellationToken ct)
    {
        await req.ExecuteAsync(ct);
    }
}

public record ArchiveCourse : ICommand
{
    public required CourseId CourseId { get; init; }
}

public class ArchiveCourseHandler : ICommandHandler<ArchiveCourse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ArchiveCourse command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);
        if (course is null)
        {
            throw new InvalidOperationException("Course not found");
        }

        course.Archive();
        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
    }
}

public class ArchiveCourseValidator : Validator<ArchiveCourse>
{
    public ArchiveCourseValidator()
    {
        _ = RuleFor(x => x.CourseId).NotEmpty();
    }
}
