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
using Microsoft.AspNetCore.Http;

namespace AcademicManagement.Application.UseCases.Courses;

public class GetAllCoursesEndpoint : Endpoint<GetCourses, IReadOnlyList<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly IUserContextService _userContextService;

    public GetAllCoursesEndpoint(ICourseRepository courseRepository, IUniversityRepository universityRepository, IProfessorRepository professorRepository, IUserContextService userContextService)
    {
        _courseRepository = courseRepository;
        _universityRepository = universityRepository;
        _professorRepository = professorRepository;
        _userContextService = userContextService;
    }

    public override void Configure()
    {
        Get("academic-management/courses");
        Policies(PolicyAcademicManagement.PresidentOnly, PolicyAcademicManagement.ProfessorOnly);
        Description(x => x.WithTags("academic-management/courses"));
    }

    public override async Task HandleAsync(GetCourses req, CancellationToken ct)
    {
        var currentUser = _userContextService.GetCurrentUser();

        if (currentUser.Role == UserRole.President)
        {
            var university = await _universityRepository.GetByIdAsync(req.UniversityId);
            var presidentId = PresidentId.From(currentUser.Id.Value);
            if (university.President != presidentId)
            {
                throw new UnauthorizedAccessException("You can only view courses from your own university");
            }
        }
        else if (currentUser.Role == UserRole.Professor)
        {
            var professor = await _professorRepository.GetByIdAsync(ProfessorId.From(currentUser.Id.Value));
            if (professor.WorkPlace != req.UniversityId)
            {
                throw new UnauthorizedAccessException("You can only view courses from your own university");
            }
        }

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
