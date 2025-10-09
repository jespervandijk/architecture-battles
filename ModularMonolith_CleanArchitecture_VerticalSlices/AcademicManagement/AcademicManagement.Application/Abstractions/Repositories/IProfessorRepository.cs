using AcademicManagement.Domain.Professors;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IProfessorRepository : IRepository<Professor, ProfessorId>
{
}
