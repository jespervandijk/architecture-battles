using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Universities;
using Marten;

namespace AcademicManagement.Infrastructure.Repositories;

public class UniversityRepository : Repository<University, UniversityId>, IUniversityRepository
{
    public UniversityRepository(IDocumentSession documentSession) : base(documentSession)
    {
    }
}
