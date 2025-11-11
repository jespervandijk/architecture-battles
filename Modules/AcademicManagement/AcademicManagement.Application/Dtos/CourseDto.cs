using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Application.Dtos;

public record CourseDto
{
    public required CourseId Id { get; init; }
    public required UniversityId University { get; init; }
    public required DepartmentId Department { get; init; }
    public required ProfessorId CourseOwner { get; init; }
    public required IReadOnlyList<ProfessorId> Professors { get; init; }
    public required Name Title { get; init; }
    public Description? Description { get; init; }
    public required Credits Credits { get; init; }
    public StudentCapacity? MaxCapacity { get; init; }
    public required CourseStatus Status { get; init; }
    public required List<SectionDto> Sections { get; init; }

    public static CourseDto FromDomain(Course course)
    {
        return new CourseDto
        {
            Id = course.Id,
            University = course.University,
            Department = course.Department,
            CourseOwner = course.CourseOwner,
            Professors = course.Professors,
            Title = course.Title,
            Description = course.Description,
            Credits = course.Credits,
            MaxCapacity = course.MaxCapacity,
            Status = course.Status,
            Sections = [.. course.Sections.Select(SectionDto.FromDomain)]
        };
    }
}
public record SectionDto
{
    public required SectionId Id { get; init; }
    public required string Name { get; init; }
    public required ProfessorId Professor { get; init; }
    public Url? TeachingMaterials { get; init; }

    public static SectionDto FromDomain(Section section)
    {
        return new SectionDto
        {
            Id = section.Id,
            Name = section.Name,
            Professor = section.Professor,
            TeachingMaterials = section.TeachingMaterials
        };
    }
}