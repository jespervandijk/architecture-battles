using AcademicManagement.Domain.Scalars;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Courses;

public record Assignment
{
    public required Name Title { get; init; }
    public required GradeWeight GradeWeight { get; init; }
    public required Uri DocumentUrl { get; init; }
    public required Year SchoolYear { get; init; }
}
