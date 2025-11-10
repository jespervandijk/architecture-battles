using Vogen;

namespace AcademicManagement.Domain.Aggregates.Assignments;

[ValueObject<Guid>]
public readonly partial struct AssignmentId
{
    public static AssignmentId Next() => From(Guid.NewGuid());
}
