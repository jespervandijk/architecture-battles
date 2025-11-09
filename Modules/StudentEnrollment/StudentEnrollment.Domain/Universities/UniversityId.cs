using Vogen;

namespace StudentEnrollment.Domain.Universities;

[ValueObject<Guid>]
public partial struct UniversityId
{
    public static UniversityId Next() => From(Guid.NewGuid());
}
