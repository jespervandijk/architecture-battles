using StudentEnrollment.Domain.Students;

namespace StudentEnrollment.Application.Abstractions.Repositories;

public interface IStudentRepository : IRepository<Student, StudentId>
{
}
