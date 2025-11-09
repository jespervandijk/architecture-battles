using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Courses;

public record Exam
{
    public required Name Title { get; init; }
    public required TimeSpan Duration { get; init; }
    public required GradeWeight GradeWeight { get; init; }
    public required string SchoolYear { get; init; }
    public required string DocumentUrl { get; init; }
}
