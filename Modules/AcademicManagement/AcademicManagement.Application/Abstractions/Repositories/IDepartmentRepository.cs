using AcademicManagement.Domain.Aggregates.Departments;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IDepartmentRepository : IRepository<Department, DepartmentId>
{
}
