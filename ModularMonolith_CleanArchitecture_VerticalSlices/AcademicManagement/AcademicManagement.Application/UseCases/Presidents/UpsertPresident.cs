using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Users;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Presidents;

public class UpsertPresidentEndpoint : Endpoint<UpsertPresident, PresidentId>
{
    public override void Configure()
    {
        Post("academic-management/presidents/upsert");
    }

    public override Task HandleAsync(UpsertPresident req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}

public record UpsertPresident : ICommand<PresidentId>
{
    public PresidentId? Id { get; init; }
    public required Name FirstName { get; init; }
    public required Name LastName { get; init; }
    public required UserName UserName { get; init; }
}

public class UpsertPresidentHandler : ICommandHandler<UpsertPresident, PresidentId>
{
    private readonly IPresidentRepository _presidentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertPresidentHandler(IPresidentRepository presidentRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _presidentRepository = presidentRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PresidentId> ExecuteAsync(UpsertPresident command, CancellationToken ct)
    {
        if (command.Id is not null)
        {
            var existingPresident = await _presidentRepository.GetByIdAsync(command.Id.Value);
            if (existingPresident is null)
            {
                throw new ArgumentException($"President with id {command.Id} was not found.");
            }
            var existingUser = await _userRepository.GetByIdAsync(existingPresident.UserId);
            if (existingUser is null)
            {
                throw new ArgumentException($"User with id {existingPresident.UserId} was not found.");
            }
            existingPresident.FirstName = command.FirstName;
            existingPresident.LastName = command.LastName;
            existingUser.Name = command.UserName;
            _presidentRepository.Update(existingPresident);
            _userRepository.Update(existingUser);
            await _unitOfWork.SaveChangesAsync();
            return existingPresident.Id;
        }
        var user = User.Create(command.UserName, UserRole.President);
        var president = President.Create(user.Id, command.FirstName, command.LastName);
        _userRepository.Insert(user);
        _presidentRepository.Insert(president);
        await _unitOfWork.SaveChangesAsync();
        return president.Id;
    }
}
