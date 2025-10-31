using AcademicManagement.Domain.Professors;
using Marten;

namespace AcademicManagement.Infrastructure.Repositories;

public class ProfessorRepository : Repository<Professor, ProfessorId>
{
    public ProfessorRepository(IDocumentSession documentSession) : base(documentSession)
    {
    }
}
