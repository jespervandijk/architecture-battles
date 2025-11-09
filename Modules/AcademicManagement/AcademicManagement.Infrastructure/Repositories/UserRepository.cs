using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Users;
using Marten;

namespace AcademicManagement.Infrastructure.Repositories;

public class UserRepository : Repository<User, UserId>, IUserRepository
{
    public UserRepository(IDocumentSession documentSession) : base(documentSession)
    {
    }
}
