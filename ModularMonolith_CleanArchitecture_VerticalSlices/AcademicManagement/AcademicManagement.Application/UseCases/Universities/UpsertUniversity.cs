using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using PresidentId = AcademicManagement.Domain.Aggregates.Presidents.PresidentId;
using UniversityId = AcademicManagement.Domain.Aggregates.Universities.UniversityId;

namespace AcademicManagement.Application.UseCases.Universities;

public class UpsertUniversityEndpoint : Endpoint<UpsertUniversity, UniversityId>
{
    public override void Configure()
    {
        Post("academic-management/university/create");
    }

    public override async Task HandleAsync(UpsertUniversity req, CancellationToken ct)
    {
        var result = await req.ExecuteAsync();
        Response = result;
    }
}

public record UpsertUniversity : ICommand<UniversityId>
{
    public UniversityId? University { get; init; }

    public required Name Name { get; init; }

    public required PresidentId President { get; init; }
}

public class UpsertUniversityHandler : ICommandHandler<UpsertUniversity, UniversityId>
{
    private readonly IUniversityRepository _universityRepository;

    public UpsertUniversityHandler(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository;
    }

    public async Task<UniversityId> ExecuteAsync(UpsertUniversity command, CancellationToken ct)
    {
        var university = University.Create(command.President, command.Name);
        _universityRepository.Upsert(university);
        return university.Id;
    }

}