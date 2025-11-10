using AcademicManagement.Domain.Aggregates.Students;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Assignments;

public record AssignmentResult
{
    public required StudentId StudentId { get; init; }
    public required Grade Grade { get; init; }
}
