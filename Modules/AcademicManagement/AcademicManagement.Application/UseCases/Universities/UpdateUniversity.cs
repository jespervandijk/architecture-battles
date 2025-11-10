using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Universities;

public class UpdateUniversityEndpoint : Endpoint<UpdateUniversity, UniversityId>
{
    public override void Configure()
    {
        Post("academic-management/university/update");
        Policies(PolicyAcademicManagement.PresidentOnly);
    }

    public override async Task HandleAsync(UpdateUniversity req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record UpdateUniversity : ICommand<UniversityId>
{
    public required UniversityId UniversityId { get; init; }
    public required Name Name { get; init; }
}

public class UpdateUniversityHandler : ICommandHandler<UpdateUniversity, UniversityId>
{
    private readonly IUniversityRepository _universityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUniversityHandler(IUniversityRepository universityRepository, IUnitOfWork unitOfWork)
    {
        _universityRepository = universityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UniversityId> ExecuteAsync(UpdateUniversity command, CancellationToken ct)
    {
        var university = await _universityRepository.GetByIdAsync(command.UniversityId) ?? throw new InvalidOperationException($"University with id {command.UniversityId} was not found.");
        university.Update(command.Name);
        _universityRepository.Update(university);
        await _unitOfWork.SaveChangesAsync();
        return university.Id;
    }
}

public class UpdateUniversityValidator : Validator<UpdateUniversity>
{
    public UpdateUniversityValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty();
        _ = RuleFor(x => x.UniversityId)
            .NotEmpty()
            .MustAsync(async (universityId, ct) =>
            {
                var universityRepo = Resolve<IUniversityRepository>();
                var university = await universityRepo.GetByIdAsync(universityId);
                return university is not null;
            })
            .WithMessage("University not found")
            .MustAsync(async (universityId, ct) =>
            {
                var universityRepo = Resolve<IUniversityRepository>();
                var userContext = Resolve<IUserContextService>();

                var university = await universityRepo.GetByIdAsync(universityId);
                var currentUser = userContext.GetCurrentUser();

                var presidentId = PresidentId.From(currentUser.Id.Value);
                return university.President == presidentId;
            })
            .WithMessage("You are not authorized to update this university");
    }
}
