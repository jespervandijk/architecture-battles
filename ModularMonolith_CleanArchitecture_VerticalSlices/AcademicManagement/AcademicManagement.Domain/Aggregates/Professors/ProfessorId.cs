
using Vogen;

namespace AcademicManagement.Domain.Aggregates.Professors;

[ValueObject<Guid>]
public partial struct ProfessorId
{
    public static ProfessorId Next() => From(Guid.NewGuid());
}
