using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Universities;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Universities;

public class ArchiveUniversityEndpoint : Endpoint<ArchiveUniversity, UniversityId>
{
    public override void Configure()
    {
        Post("academic-management/university/archive");
        Policies(PolicyAcademicManagement.PresidentOnly);
    }

    public override async Task HandleAsync(ArchiveUniversity req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record ArchiveUniversity : ICommand<UniversityId>
{
    public required UniversityId UniversityId { get; init; }
}

public class ArchiveUniversityHandler : ICommandHandler<ArchiveUniversity, UniversityId>
{
    private readonly IUniversityRepository _universityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveUniversityHandler(IUniversityRepository universityRepository, IUnitOfWork unitOfWork)
    {
        _universityRepository = universityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UniversityId> ExecuteAsync(ArchiveUniversity command, CancellationToken ct)
    {
        var university = await _universityRepository.GetByIdAsync(command.UniversityId);
        if (university == null)
        {
            throw new InvalidOperationException($"University with id {command.UniversityId} not found.");
        }

        university.IsArchived = true;
        _universityRepository.Update(university);
        await _unitOfWork.SaveChangesAsync();

        return university.Id;
    }
}

public class ArchiveUniversityValidator : Validator<ArchiveUniversity>
{
    public ArchiveUniversityValidator()
    {
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
            .WithMessage("You are not authorized to archive this university");
    }
}


