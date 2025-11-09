using Vogen;

namespace AcademicManagement.Domain.Aggregates.Courses;

[ValueObject<Guid>]
public readonly partial struct SectionId
{
    public static SectionId Next() => From(Guid.NewGuid());
}
