using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Departments;
using Marten;

namespace AcademicManagement.Infrastructure.Repositories;

public class DepartmentRepository : Repository<Department, DepartmentId>, IDepartmentRepository
{
    public DepartmentRepository(IDocumentSession documentSession) : base(documentSession)
    {
    }
}
