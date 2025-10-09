using StudentEnrollment.Domain.Courses;
using StudentEnrollment.Domain.Students;

namespace StudentEnrollment.Domain.Universities;

public class University
{
    public UniversityId Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public List<StudentId> StudentIds { get; private set; }

    public List<CourseId> CourseIds { get; private set; }

    public University(string name, string description)
    {
        Id = UniversityId.Next();
        Name = name;
        Description = description;
        StudentIds = [];
        CourseIds = [];
    }
}
