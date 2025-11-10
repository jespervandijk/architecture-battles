using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Application.Dtos;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Aggregates.Users;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Courses;

public class GetAllCoursesEndpoint : Endpoint<GetCourses, IReadOnlyList<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;

    public GetAllCoursesEndpoint(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public override void Configure()
    {
        Get("academic-management/courses");
        Policies(PolicyAcademicManagement.PresidentOnly, PolicyAcademicManagement.ProfessorOnly);
    }

    public override async Task HandleAsync(GetCourses req, CancellationToken ct)
    {
        var courses = await _courseRepository.GetAllAsync();
        var filteredCourses = courses.Where(course =>
            course.University == req.UniversityId &&
            (req.DepartmentId == null || course.Department == req.DepartmentId) &&
            (req.CourseOwnerId == null || course.CourseOwner == req.CourseOwnerId) &&
            (req.ProfessorId == null || course.Professors.Contains(req.ProfessorId.Value)) &&
            (req.Title == null || course.Title.Value.Contains(req.Title.Value.Value, StringComparison.OrdinalIgnoreCase))
        );
        Response = [.. filteredCourses.Select(CourseDto.FromDomain)];
    }
}

public record GetCourses
{
    public required UniversityId UniversityId { get; init; }
    public DepartmentId? DepartmentId { get; init; }
    public ProfessorId? CourseOwnerId { get; init; }
    public ProfessorId? ProfessorId { get; init; }
    public Name? Title { get; init; }
}

public class GetCoursesValidator : Validator<GetCourses>
{
    public GetCoursesValidator()
    {
        _ = RuleFor(x => x.UniversityId)
            .NotEmpty()
            .MustAsync(async (universityId, ct) =>
            {
                var userContext = Resolve<IUserContextService>();
                var currentUser = userContext.GetCurrentUser();

                if (currentUser.Role == UserRole.President)
                {
                    var universityRepo = Resolve<IUniversityRepository>();
                    var university = await universityRepo.GetByIdAsync(universityId);
                    var presidentId = PresidentId.From(currentUser.Id.Value);
                    return university?.President == presidentId;
                }
                else if (currentUser.Role == UserRole.Professor)
                {
                    var professorRepo = Resolve<IProfessorRepository>();
                    var professor = await professorRepo.GetByIdAsync(ProfessorId.From(currentUser.Id.Value));
                    return professor?.WorkPlace == universityId;
                }

                return false;
            })
            .WithMessage("You can only view courses from your own university");
    }
}
