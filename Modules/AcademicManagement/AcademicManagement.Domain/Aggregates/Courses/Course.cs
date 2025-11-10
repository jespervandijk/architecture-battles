using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Courses;

public sealed class Course
{
    public CourseId Id { get; init; }
    public UniversityId University { get; set; }
    public DepartmentId Department { get; set; }
    public ProfessorId CourseOwner { get; set; }
    public List<ProfessorId> Professors { get; set; }
    public Name Title { get; set; }
    public Description? Description { get; set; }
    public Credits Credits { get; set; }
    public StudentCapacity? MaxCapacity { get; set; }
    public CourseStatus Status { get; set; }
    public bool IsArchived { get; set; }
    public List<Section> Sections { get; set; }

    [JsonConstructor]
    private Course(
        CourseId id,
        UniversityId university,
        DepartmentId department,
        ProfessorId courseOwner,
        List<ProfessorId> professors,
        Name title,
        Description? description,
        Credits credits,
        StudentCapacity? maxCapacity,
        CourseStatus status,
        bool isArchived,
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
        IsArchived = isArchived;
        Sections = sections;
    }

    public static Course Create(
        UniversityId university,
        DepartmentId department,
        ProfessorId courseOwner,
        Name title,
        Credits credits,
        Description? description = null,
        StudentCapacity? maxCapacity = null
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
            false,
            []
        );
    }

    public void UpdateCourseDetails(Name title, Description? description, StudentCapacity? maxCapacity)
    {
        Title = title;
        Description = description;
        MaxCapacity = maxCapacity;
    }

    public void UpdateCourse(ProfessorId courseOwner, Name title, Description? description, Credits credits, StudentCapacity? maxCapacity)
    {
        CourseOwner = courseOwner;
        Title = title;
        Description = description;
        Credits = credits;
        MaxCapacity = maxCapacity;
    }

    public void Archive()
    {
        IsArchived = true;
    }

    public void AssignProfessorAsCourseOwner(ProfessorId professorId)
    {
        CourseOwner = professorId;
        if (!Professors.Contains(professorId))
        {
            Professors.Add(professorId);
        }
    }

    public void AssignProfessor(ProfessorId professorId)
    {
        if (!Professors.Contains(professorId))
        {
            Professors.Add(professorId);
        }
    }

    public void RemoveProfessor(ProfessorId professorId)
    {
        if (professorId == CourseOwner)
        {
            throw new InvalidOperationException("Cannot remove the course owner. Please assign a new course owner first.");
        }

        _ = Professors.Remove(professorId);
    }

    public void CreateSection(string name, ProfessorId professor)
    {
        var section = Section.Create(name, professor);
        Sections.Add(section);
    }

    public void UpdateSection(SectionId sectionId, string name, ProfessorId professor)
    {
        var section = Sections.FirstOrDefault(s => s.Id == sectionId) ?? throw new InvalidOperationException($"Section with id {sectionId} not found in this course.");
        section.Update(name, professor);
    }

    public void UpdateSectionDetails(SectionId sectionId, string name, Uri? teachingMaterialsUrl)
    {
        var section = Sections.FirstOrDefault(s => s.Id == sectionId) ?? throw new InvalidOperationException($"Section with id {sectionId} not found in this course.");
        section.UpdateDetails(name, teachingMaterialsUrl);
    }

    public void Validate()
    {
        CourseValidator.Validate(this);
    }

    private static class CourseValidator
    {
        private const int MaxSections = 4;

        public static void Validate(Course course)
        {
            ValidateSectionCount(course.Sections.Count);
        }

        private static void ValidateSectionCount(int currentCount)
        {
            if (currentCount >= MaxSections)
            {
                throw new InvalidOperationException($"A course cannot have more than {MaxSections} sections. Each section takes up a school period and there are only {MaxSections} periods in a year.");
            }
        }
    }
}
