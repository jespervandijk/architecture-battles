using Vogen;

namespace AcademicManagement.Domain.Aggregates.Courses;

[ValueObject<Guid>]
public partial struct CourseId
{
    public static CourseId Next() => From(Guid.NewGuid());
}
