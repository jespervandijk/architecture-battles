using AcademicManagement.Domain.Aggregates.Presidents;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IPresidentRepository : IRepository<President, PresidentId>
{
}
