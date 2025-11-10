using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Assignments;
using Marten;

namespace AcademicManagement.Infrastructure.Repositories;

public sealed class AssignmentRepository : Repository<Assignment, AssignmentId>, IAssignmentRepository
{
    public AssignmentRepository(IDocumentSession session) : base(session)
    {
    }
}
