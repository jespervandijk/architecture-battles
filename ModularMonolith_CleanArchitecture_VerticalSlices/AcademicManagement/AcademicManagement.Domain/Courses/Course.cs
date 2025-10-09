namespace AcademicManagement.Domain.Courses;

public class Course
{
    public CourseId Id { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public Credits Credits { get; private set; }

    public int MaxCapacity { get; private set; }

    public CourseStatus Status { get; private set; }

    public Course(string title, string description, Credits credits, int maxCapacity, CourseStatus status)
    {
        Id = CourseId.Next();
        Title = title;
        Description = description;
        Credits = credits;
        MaxCapacity = maxCapacity;
        Status = status;
    }
}
