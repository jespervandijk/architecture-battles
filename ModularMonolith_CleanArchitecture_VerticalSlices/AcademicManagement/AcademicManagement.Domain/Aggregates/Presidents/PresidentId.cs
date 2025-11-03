using Vogen;

namespace AcademicManagement.Domain.Aggregates.Presidents;

[ValueObject<Guid>]
public partial struct PresidentId
{
    public static PresidentId Next() => From(Guid.NewGuid());
}
