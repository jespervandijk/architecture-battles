using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Qowaiv;

namespace AcademicManagement.Application.UseCases.Professors;

public class UpdateProfessorEndpoint : Endpoint<UpdateProfessor, ProfessorId>
{
    public override void Configure()
    {
        Post("academic-management/professors/update");
        Policies(PolicyAcademicManagement.PresidentOnly);
        Description(x => x.WithTags("academic-management/professors"));
    }

    public override async Task HandleAsync(UpdateProfessor req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record UpdateProfessor : ICommand<ProfessorId>
{
    public required ProfessorId ProfessorId { get; init; }
    public required Name FirstName { get; init; }
    public required Name LastName { get; init; }
    public required EmailAddress EmailAddress { get; init; }
    public required Rank Rank { get; init; }
}

public class UpdateProfessorHandler : ICommandHandler<UpdateProfessor, ProfessorId>
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateProfessorHandler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork, IUniversityRepository universityRepository, IUserContextService userContextService)
    {
        _professorRepository = professorRepository;
        _unitOfWork = unitOfWork;
        _universityRepository = universityRepository;
        _userContextService = userContextService;
    }

    public async Task<ProfessorId> ExecuteAsync(UpdateProfessor command, CancellationToken ct)
    {
        var professor = await _professorRepository.GetByIdAsync(command.ProfessorId);
        var university = await _universityRepository.GetByIdAsync(professor.WorkPlace);

        var presidentId = _userContextService.GetPresidentId();
        if (university.President != presidentId)
        {
            throw new UnauthorizedAccessException("You must be the president of the university where this professor works");
        }

        professor.Update(command.FirstName, command.LastName, command.EmailAddress, command.Rank);
        _professorRepository.Update(professor);
        await _unitOfWork.SaveChangesAsync();
        return professor.Id;
    }
}
