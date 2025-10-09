using AcademicManagement.Domain.Professors;

namespace AcademicManagement.Application.Abstractions;

public interface IProfessorRepository : IRepository<Professor, ProfessorId>
{
}
