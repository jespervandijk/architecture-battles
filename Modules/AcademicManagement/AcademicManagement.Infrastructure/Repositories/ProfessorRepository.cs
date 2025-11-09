using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Professors;
using Marten;

namespace AcademicManagement.Infrastructure.Repositories;

public class ProfessorRepository : Repository<Professor, ProfessorId>, IProfessorRepository
{
    public ProfessorRepository(IDocumentSession documentSession) : base(documentSession)
    {
    }
}
