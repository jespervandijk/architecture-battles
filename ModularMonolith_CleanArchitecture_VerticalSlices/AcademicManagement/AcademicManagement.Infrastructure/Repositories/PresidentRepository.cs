using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Presidents;
using Marten;

namespace AcademicManagement.Infrastructure.Repositories;

public class PresidentRepository : Repository<President, PresidentId>, IPresidentRepository
{
    public PresidentRepository(IDocumentSession documentSession) : base(documentSession)
    {
    }
}
