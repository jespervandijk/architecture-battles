using AcademicManagement.Domain.Aggregates.Professors;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IProfessorRepository : IRepository<Professor, ProfessorId>
{
}
