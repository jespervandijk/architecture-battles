using Vogen;

namespace AcademicManagement.Domain.Aggregates.Users;

[ValueObject<Guid>]
public readonly partial struct UserId
{
    public static UserId Next() => From(Guid.NewGuid());
}
