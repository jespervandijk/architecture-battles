using AcademicManagement.Domain.GeneralValueObjects.Users;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
}
