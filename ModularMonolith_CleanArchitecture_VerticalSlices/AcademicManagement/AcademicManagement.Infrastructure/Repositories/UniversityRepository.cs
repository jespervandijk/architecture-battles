using AcademicManagement.Domain.Universities;
using Marten;

namespace AcademicManagement.Infrastructure.Repositories;

public class UniversityRepository : Repository<University, UniversityId>
{
    public UniversityRepository(IDocumentSession documentSession) : base(documentSession)
    {
    }
}
