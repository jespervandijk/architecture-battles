using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
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
    private readonly IUserContextService _userContextService;

    public ArchiveUniversityHandler(IUniversityRepository universityRepository, IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _universityRepository = universityRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<UniversityId> ExecuteAsync(ArchiveUniversity command, CancellationToken ct)
    {
        var university = await _universityRepository.GetByIdAsync(command.UniversityId);

        var presidentId = _userContextService.GetPresidentId();
        if (university.President != presidentId)
        {
            throw new UnauthorizedAccessException("You must be the president of this university.");
        }

        university.Archive();
        _universityRepository.Update(university);
        await _unitOfWork.SaveChangesAsync();

        return university.Id;
    }
}
