using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Presidents;

public class UpdatePresidentEndpoint : Endpoint<UpdatePresident, PresidentId>
{
    public override void Configure()
    {
        Post("academic-management/presidents/update");
        Policies(PolicyAcademicManagement.PresidentOnly);
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

    public UpdatePresidentHandler(IPresidentRepository presidentRepository, IUnitOfWork unitOfWork)
    {
        _presidentRepository = presidentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PresidentId> ExecuteAsync(UpdatePresident command, CancellationToken ct)
    {
        var president = await _presidentRepository.GetByIdAsync(command.PresidentId) ?? throw new InvalidOperationException($"President with id {command.PresidentId} was not found.");
        president.Update(command.FirstName, command.LastName);
        _presidentRepository.Update(president);
        await _unitOfWork.SaveChangesAsync();
        return president.Id;
    }
}

public class UpdatePresidentValidator : Validator<UpdatePresident>
{
    public UpdatePresidentValidator()
    {
        _ = RuleFor(x => x.FirstName).NotEmpty();
        _ = RuleFor(x => x.LastName).NotEmpty();
        _ = RuleFor(x => x.PresidentId)
            .NotEmpty()
            .MustAsync(async (presidentId, ct) =>
            {
                var presidentRepo = Resolve<IPresidentRepository>();
                var president = await presidentRepo.GetByIdAsync(presidentId);
                return president is not null;
            })
            .WithMessage("President not found")
            .MustAsync(async (presidentId, ct) =>
            {
                var presidentRepo = Resolve<IPresidentRepository>();
                var userContext = Resolve<IUserContextService>();

                var president = await presidentRepo.GetByIdAsync(presidentId);
                var currentUser = userContext.GetCurrentUser();

                var currentPresidentId = PresidentId.From(currentUser.Id.Value);
                return president.Id == currentPresidentId;
            })
            .WithMessage("You can only update your own president profile");
    }
}
