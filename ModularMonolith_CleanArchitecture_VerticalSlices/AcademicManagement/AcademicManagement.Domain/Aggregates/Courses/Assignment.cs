using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Courses;

public record Assignment
{
    public required Name Title { get; init; }
    public required GradeWeight GradeWeight { get; init; }
    public required string DocumentUrl { get; init; }
    public required string SchoolYear { get; init; }
}
