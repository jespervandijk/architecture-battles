using Vogen;

namespace AcademicManagement.Domain.Aggregates.Students;

[ValueObject<Guid>]
public readonly partial struct StudentId
{
    public static StudentId Next() => From(Guid.NewGuid());
}
