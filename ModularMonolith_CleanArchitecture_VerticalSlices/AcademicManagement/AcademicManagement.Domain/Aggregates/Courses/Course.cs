using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;

namespace AcademicManagement.Domain.Aggregates.Courses;

public sealed class Course
{
    public CourseId Id { get; init; }

    public UniversityId University { get; set; }

    public DepartmentId Department { get; set; }

    public ProfessorId CourseOwner { get; set; }

    public List<ProfessorId> Professors { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public Credits Credits { get; set; }

    public int? MaxCapacity { get; set; }

    public CourseStatus Status { get; set; }

    public List<Section> Sections { get; set; }

    [JsonConstructor]
    private Course(
        CourseId id,
        UniversityId university,
        DepartmentId department,
        ProfessorId courseOwner,
        List<ProfessorId> professors,
        string title,
        string? description,
        Credits credits,
        int? maxCapacity,
        CourseStatus status,
        List<Section> sections
        )
    {
        Id = id;
        University = university;
        Department = department;
        CourseOwner = courseOwner;
        Professors = professors;
        Title = title;
        Description = description;
        Credits = credits;
        MaxCapacity = maxCapacity;
        Status = status;
        Sections = sections;
    }

    public static Course Create(
        UniversityId university,
        DepartmentId department,
        ProfessorId courseOwner,
        string title,
        Credits credits,
        string? description = null,
        int? maxCapacity = null
    )
    {
        return new Course(
            CourseId.Next(),
            university,
            department,
            courseOwner,
            [],
            title,
            description,
            credits,
            maxCapacity,
            CourseStatus.Active,
            []
        );
    }
}
