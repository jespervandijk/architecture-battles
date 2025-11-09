using Qowaiv;

namespace StudentEnrollment.Domain.Enrollments;

public record Assignment
{
    public required Grade Grade { get; init; }

    public required Percentage PercentageOfFinalGrade { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required DateTime DeliveredOn { get; init; }
}
