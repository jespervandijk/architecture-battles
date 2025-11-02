using AcademicManagement.Domain.Aggregates.Universities;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IUniversityRepository : IRepository<University, UniversityId>
{
}
