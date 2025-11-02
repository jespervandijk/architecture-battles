using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Universities;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Universities;

public class DeleteUniversityEndpoint : Endpoint<DeleteUniversity, UniversityId>
{
    public override void Configure()
    {
        Delete("academic-management/university/delete");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteUniversity req, CancellationToken ct)
    {
        var result = await req.ExecuteAsync();
        Response = result;
    }
}
public record DeleteUniversity : ICommand<UniversityId>
{
    public required UniversityId UniversityId { get; init; }
}

public class DeleteUniversityHandler : ICommandHandler<DeleteUniversity, UniversityId>
{
    private readonly IUniversityRepository _universityRepository;

    public DeleteUniversityHandler(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository;
    }

    public async Task<UniversityId> ExecuteAsync(DeleteUniversity command, CancellationToken ct)
    {
        await _universityRepository.Delete(command.UniversityId);
        return command.UniversityId;
    }

}


