using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace AcademicManagement.Application.UseCases.Universities;

public class CreateUniversityEndpoint : Endpoint<CreateUniversity, UniversityId>
{
    public override void Configure()
    {
        Post("academic-management/universities/create");
        Policies(PolicyAcademicManagement.PresidentOnly);
        Description(x => x.WithTags("academic-management/universities"));
    }

    public override async Task HandleAsync(CreateUniversity req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record CreateUniversity : ICommand<UniversityId>
{
    public required Name Name { get; init; }
    public required PresidentId President { get; init; }
}

public class CreateUniversityHandler : ICommandHandler<CreateUniversity, UniversityId>
{
    private readonly IUniversityRepository _universityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUniversityHandler(IUniversityRepository universityRepository, IUnitOfWork unitOfWork)
    {
        _universityRepository = universityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UniversityId> ExecuteAsync(CreateUniversity command, CancellationToken ct)
    {
        var university = University.Create(command.President, command.Name);
        _universityRepository.Insert(university);
        await _unitOfWork.SaveChangesAsync();
        return university.Id;
    }
}
