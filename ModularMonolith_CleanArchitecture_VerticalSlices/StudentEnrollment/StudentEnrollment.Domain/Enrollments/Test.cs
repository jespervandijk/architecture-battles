using Qowaiv;

namespace StudentEnrollment.Domain.Enrollments;

public record Test
{
    public required DateTime DateTime { get; init; }

    public required Grade Grade { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required Percentage PercentageOfFinalGrade { get; init; }
}
