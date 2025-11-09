
using StudentEnrollment.Domain.Courses;
using StudentEnrollment.Domain.Students;

namespace StudentEnrollment.Domain.Enrollments;

public class Enrollment
{
    public EnrollmentId Id { get; private set; }

    public StudentId StudentId { get; private set; }

    public CourseId CourseId { get; private set; }

    public List<Assignment> Assignments { get; private set; }

    public List<Test> Tests { get; private set; }

    public Enrollment(StudentId student, CourseId course)
    {
        Id = EnrollmentId.Next();
        StudentId = student;
        CourseId = course;
        Assignments = [];
        Tests = [];
    }
}
