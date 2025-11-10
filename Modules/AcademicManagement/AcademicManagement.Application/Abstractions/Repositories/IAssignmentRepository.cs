using AcademicManagement.Domain.Aggregates.Assignments;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IAssignmentRepository : IRepository<Assignment, AssignmentId>
{
}
