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
    public ProfessorId CourseOwner { get; internal set; }
    private readonly List<ProfessorId> _professors;
    public IReadOnlyList<ProfessorId> Professors => _professors;
    public Name Title { get; private set; }
    public Description? Description { get; private set; }
    public Credits Credits { get; private set; }
    public StudentCapacity? MaxCapacity { get; private set; }
    public CourseStatus Status { get; set; }
    private readonly List<Section> _sections;
    public IReadOnlyList<Section> Sections => _sections;

    [JsonConstructor]
    internal Course(
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
        List<Section> sections
        )
    {
        Id = id;
        University = university;
        Department = department;
        CourseOwner = courseOwner;
        _professors = professors;
        Title = title;
        Description = description;
        Credits = credits;
        MaxCapacity = maxCapacity;
        Status = status;
        _sections = sections;
    }


    public void UpdateCourseDetails(Name title, Description? description, StudentCapacity? maxCapacity)
    {
        Title = title;
        Description = description;
        MaxCapacity = maxCapacity;
    }

    public void ChangeCredits(Credits credits)
    {
        Credits = credits;
    }

    public void Archive()
    {
        Status = CourseStatus.Archived;
    }

    internal void AddProfessor(ProfessorId professorId)
    {
        if (!_professors.Contains(professorId))
        {
            _professors.Add(professorId);
        }
    }

    public void RemoveProfessor(ProfessorId professorId)
    {
        if (professorId == CourseOwner)
        {
            throw new InvalidOperationException("Cannot remove the course owner. Please assign a new course owner first.");
        }

        _ = _professors.Remove(professorId);
    }

    public SectionId CreateSection(string name, ProfessorId professorId)
    {
        if (Professors.Contains(professorId))
        {
            throw new InvalidOperationException("Professor must be assigned to the course before being assigned to a section.");
        }
        var section = Section.Create(name, professorId);
        _sections.Add(section);
        return section.Id;
    }

    public void ChangeSectionProfessor(SectionId sectionId, ProfessorId professorId)
    {
        if (!Professors.Contains(professorId))
        {
            throw new InvalidOperationException("Professor must be assigned to the course before being assigned to a section.");
        }
        var section = Sections.FirstOrDefault(s => s.Id == sectionId) ?? throw new InvalidOperationException($"Section with id {sectionId} not found in this course.");

        section.Professor = professorId;
    }

    public void UpdateSectionDetails(SectionId sectionId, string name, Url? teachingMaterialsUrl)
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
