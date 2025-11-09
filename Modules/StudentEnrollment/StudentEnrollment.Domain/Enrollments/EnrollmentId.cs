using Vogen;

namespace StudentEnrollment.Domain.Enrollments;

[ValueObject<Guid>]
public partial struct EnrollmentId
{
    public static EnrollmentId Next() => From(Guid.NewGuid());
}
