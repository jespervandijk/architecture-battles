using AcademicManagement.Domain.Universities;

namespace AcademicManagement.Application.Abstractions;

public interface IUniversityRepository : IRepository<University, UniversityId>
{
}
