using StudentEnrollment.Domain.Enrollments;

namespace StudentEnrollment.Application.Abstractions.Repositories;

public interface IEnrollmentRepository : IRepository<Enrollment, EnrollmentId>
{
}
