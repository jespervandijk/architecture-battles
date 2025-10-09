using Qowaiv;
using StudentEnrollment.Domain.Enrollments;

namespace StudentEnrollment.Domain.Courses;

public class Course
{
    public CourseId Id { get; private set; }

    public Year Year { get; private set; }

    public Semester Semester { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }


    public Course(Year year, Semester semester, string title, string description)
    {
        Id = CourseId.Next();
        Year = year;
        Semester = semester;
        Title = title;
        Description = description;
    }
}
