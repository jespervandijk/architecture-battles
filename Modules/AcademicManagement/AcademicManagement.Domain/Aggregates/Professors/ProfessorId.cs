
using Vogen;

namespace AcademicManagement.Domain.Aggregates.Professors;

[ValueObject<Guid>]
public readonly partial struct ProfessorId
{
    public static ProfessorId Next() => From(Guid.NewGuid());
}
