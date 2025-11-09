using AcademicManagement.Domain.Aggregates.Users;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
}
