using Vogen;

namespace AcademicManagement.Domain.Aggregates.Universities;

[ValueObject<Guid>]
public partial struct UniversityId
{
    public static UniversityId Next() => From(Guid.NewGuid());
}
