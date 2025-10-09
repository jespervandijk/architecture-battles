using StudentEnrollment.Domain.Universities;

namespace StudentEnrollment.Application.Abstractions.Repositories;

public interface IUniversityRepository : IRepository<University, UniversityId>
{
}
