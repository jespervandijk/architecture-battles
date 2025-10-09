namespace AcademicManagement.Domain.Courses;

public class Course
{
    public CourseId Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public required Credits Credits { get; set; }

    public required int MaxCapacity { get; set; }

    public CourseStatus Status { get; set; }

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
