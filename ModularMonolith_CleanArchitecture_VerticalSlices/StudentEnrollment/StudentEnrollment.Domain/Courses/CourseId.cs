using Vogen;

namespace StudentEnrollment.Domain.Courses;

[ValueObject<Guid>]
public partial struct CourseId
{
    public static CourseId Next() => From(Guid.NewGuid());
}
