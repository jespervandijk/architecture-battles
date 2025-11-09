using Vogen;

namespace AcademicManagement.Domain.Aggregates.Courses;

[ValueObject<Guid>]
public readonly partial struct CourseId
{
    public static CourseId Next() => From(Guid.NewGuid());
}
