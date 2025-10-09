using Vogen;

namespace AcademicManagement.Domain.Courses;

[ValueObject<Guid>]
public partial struct CourseId
{
    public static CourseId Next() => From(Guid.NewGuid());
}
