using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Users;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Presidents;

public class CreatePresidentEndpoint : Endpoint<CreatePresident, PresidentId>
{
    public override void Configure()
    {
        Post("academic-management/presidents/create");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreatePresident req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record CreatePresident : ICommand<PresidentId>
{
    public required Name FirstName { get; init; }
    public required Name LastName { get; init; }
    public required UserName UserName { get; init; }
}

public class CreatePresidentHandler : ICommandHandler<CreatePresident, PresidentId>
{
    private readonly IPresidentRepository _presidentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePresidentHandler(IPresidentRepository presidentRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _presidentRepository = presidentRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PresidentId> ExecuteAsync(CreatePresident command, CancellationToken ct)
    {
        var user = User.Create(command.UserName, UserRole.President);
        var president = President.Create(user.Id, command.FirstName, command.LastName);
        _userRepository.Insert(user);
        _presidentRepository.Insert(president);
        await _unitOfWork.SaveChangesAsync();
        return president.Id;
    }
}
