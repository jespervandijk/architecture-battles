using System.Text.Json.Serialization;
using AcademicManagement.Domain.Departments;
using AcademicManagement.Domain.Professors;
using AcademicManagement.Domain.Universities;

namespace AcademicManagement.Domain.Courses;

public class Course
{
    public CourseId Id { get; private set; }

    public UniversityId University { get; set; }

    public DepartmentId Department { get; set; }

    public ProfessorId CourseOwner { get; set; }

    public List<ProfessorId> Professors { get; set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public Credits Credits { get; private set; }

    public int MaxCapacity { get; private set; }

    public CourseStatus Status { get; private set; }

    public List<Section> Sections { get; set; }

    [JsonConstructor]
    private Course() { }

    private Course(string title, string description, Credits credits, int maxCapacity, CourseStatus status)
    {
        Id = CourseId.Next();
        Title = title;
        Description = description;
        Credits = credits;
        MaxCapacity = maxCapacity;
        Status = status;
    }

    public static Course Create(string title, string description, Credits credits, int maxCapacity, CourseStatus status)
    {
        return new Course(title, description, credits, maxCapacity, status);
    }
}
