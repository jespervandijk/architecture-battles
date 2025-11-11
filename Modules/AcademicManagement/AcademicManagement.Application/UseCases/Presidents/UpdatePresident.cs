using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace AcademicManagement.Application.UseCases.Presidents;

public class UpdatePresidentEndpoint : Endpoint<UpdatePresident, PresidentId>
{
    public override void Configure()
    {
        Post("academic-management/presidents/update");
        Policies(PolicyAcademicManagement.PresidentOnly);
        Description(x => x.WithTags("academic-management/presidents"));
    }

    public override async Task HandleAsync(UpdatePresident req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record UpdatePresident : ICommand<PresidentId>
{
    public required PresidentId PresidentId { get; init; }
    public required Name FirstName { get; init; }
    public required Name LastName { get; init; }
}

public class UpdatePresidentHandler : ICommandHandler<UpdatePresident, PresidentId>
{
    private readonly IPresidentRepository _presidentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdatePresidentHandler(IPresidentRepository presidentRepository, IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _presidentRepository = presidentRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<PresidentId> ExecuteAsync(UpdatePresident command, CancellationToken ct)
    {
        var president = await _presidentRepository.GetByIdAsync(command.PresidentId);

        var presidentId = _userContextService.GetPresidentId();
        if (president.Id != presidentId)
        {
            throw new UnauthorizedAccessException("You can only update your own president profile");
        }

        president.Update(command.FirstName, command.LastName);
        _presidentRepository.Update(president);
        await _unitOfWork.SaveChangesAsync();
        return president.Id;
    }
}
