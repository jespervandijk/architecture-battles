using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Courses;

public class UpdateCourseDetailsByCourseOwnerEndpoint : Endpoint<UpdateCourseDetailsByCourseOwner, CourseId>
{
    public override void Configure()
    {
        Post("academic-management/course/update-details");
        Policies(PolicyAcademicManagement.ProfessorOnly);
    }

    public override async Task HandleAsync(UpdateCourseDetailsByCourseOwner req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record UpdateCourseDetailsByCourseOwner : ICommand<CourseId>
{
    public required CourseId CourseId { get; init; }
    public required Name Title { get; init; }
    public Description? Description { get; init; }
    public StudentCapacity? MaxCapacity { get; init; }
}

public class UpdateCourseDetailsByCourseOwnerHandler : ICommandHandler<UpdateCourseDetailsByCourseOwner, CourseId>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateCourseDetailsByCourseOwnerHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<CourseId> ExecuteAsync(UpdateCourseDetailsByCourseOwner command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);

        var professorId = _userContextService.GetProfessorId();
        if (course.CourseOwner != professorId)
        {
            throw new UnauthorizedAccessException("You must be the course owner.");
        }

        course.UpdateCourseDetails(command.Title, command.Description, command.MaxCapacity);

        _courseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync();
        return course.Id;
    }
}
