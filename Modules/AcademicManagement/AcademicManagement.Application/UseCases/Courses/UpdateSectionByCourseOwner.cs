using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace AcademicManagement.Application.UseCases.Courses;

public class UpdateSectionByCourseOwnerEndpoint : Endpoint<UpdateSectionByCourseOwner>
{
    public override void Configure()
    {
        Post("academic-management/courses/update-section-by-course-owner");
        Policies(PolicyAcademicManagement.ProfessorOnly);
        Description(x => x.WithTags("academic-management/courses"));
    }

    public override async Task HandleAsync(UpdateSectionByCourseOwner req, CancellationToken ct)
    {
        await req.ExecuteAsync(ct);
    }
}

public record UpdateSectionByCourseOwner : ICommand
{
    public required SectionId SectionId { get; init; }
    public required CourseId CourseId { get; init; }
    public required string Name { get; init; }
    public required ProfessorId ProfessorId { get; init; }
    public required Url? TeachingMaterials { get; set; }
}

public class UpdateSectionByCourseOwnerHandler : ICommandHandler<UpdateSectionByCourseOwner>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateSectionByCourseOwnerHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task ExecuteAsync(UpdateSectionByCourseOwner command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);

        var currentProfessorId = _userContextService.GetProfessorId();
        if (course.CourseOwner != currentProfessorId)
        {
            throw new UnauthorizedAccessException("You must be the course owner");
        }

        course.UpdateSectionDetails(command.SectionId, command.Name, command.TeachingMaterials);
        course.ChangeSectionProfessor(command.SectionId, command.ProfessorId);

        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
    }
}
