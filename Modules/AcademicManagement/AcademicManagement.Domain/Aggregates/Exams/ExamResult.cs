using AcademicManagement.Domain.Aggregates.Students;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Exams;

public record class ExamResult
{
    public required StudentId StudentId { get; init; }
    public required Grade Grade { get; init; }

}
