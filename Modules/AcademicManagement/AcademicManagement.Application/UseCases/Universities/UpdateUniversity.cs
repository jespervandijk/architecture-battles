using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
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
    private readonly IUserContextService _userContextService;

    public UpdateUniversityHandler(IUniversityRepository universityRepository, IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _universityRepository = universityRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<UniversityId> ExecuteAsync(UpdateUniversity command, CancellationToken ct)
    {
        var university = await _universityRepository.GetByIdAsync(command.UniversityId);

        var presidentId = _userContextService.GetPresidentId();
        if (university.President != presidentId)
        {
            throw new UnauthorizedAccessException("You must be the president of this university.");
        }

        university.Update(command.Name);
        _universityRepository.Update(university);
        await _unitOfWork.SaveChangesAsync();
        return university.Id;
    }
}
