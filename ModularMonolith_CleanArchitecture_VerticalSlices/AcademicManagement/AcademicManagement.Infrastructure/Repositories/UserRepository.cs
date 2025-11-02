using System.Text.Json.Serialization;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.GeneralValueObjects.Users;

namespace AcademicManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IPresidentRepository _presidentRepository;
    private readonly IProfessorRepository _professorRepository;

    [JsonConstructor]
    private UserRepository(IPresidentRepository presidentRepository, IProfessorRepository professorRepository)
    {
        _presidentRepository = presidentRepository;
        _professorRepository = professorRepository;
    }

    public async Task<List<User>> GetAllUsers()
    {
        var users = new List<User>();

        var presidents = await _presidentRepository.GetAll();
        users.AddRange(presidents.Select(p => p.UserData));

        var professors = await _professorRepository.GetAll();
        users.AddRange(professors.Select(p => p.UserData));

        return users;
    }
}
