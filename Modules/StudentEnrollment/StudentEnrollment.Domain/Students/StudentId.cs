
using Vogen;

namespace StudentEnrollment.Domain.Students;

[ValueObject<Guid>]
public partial struct StudentId
{
    public static StudentId Next() => From(Guid.NewGuid());
}
