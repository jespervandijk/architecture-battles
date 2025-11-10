using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;
using FluentValidation;
using Qowaiv;

namespace AcademicManagement.Application.UseCases.Professors;

public class UpdateProfessorEndpoint : Endpoint<UpdateProfessor, ProfessorId>
{
    public override void Configure()
    {
        Post("academic-management/professor/update");
        Policies(PolicyAcademicManagement.PresidentOnly);
    }

    public override async Task HandleAsync(UpdateProfessor req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record UpdateProfessor : ICommand<ProfessorId>
{
    public required ProfessorId ProfessorId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required EmailAddress EmailAddress { get; init; }
    public required Rank Rank { get; init; }
}

public class UpdateProfessorHandler : ICommandHandler<UpdateProfessor, ProfessorId>
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfessorHandler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork)
    {
        _professorRepository = professorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfessorId> ExecuteAsync(UpdateProfessor command, CancellationToken ct)
    {
        var professor = await _professorRepository.GetByIdAsync(command.ProfessorId) ?? throw new InvalidOperationException($"Professor with id {command.ProfessorId} was not found.");
        professor.Update(command.FirstName, command.LastName, command.EmailAddress, command.Rank);
        _professorRepository.Update(professor);
        await _unitOfWork.SaveChangesAsync();
        return professor.Id;
    }
}

public class UpdateProfessorValidator : Validator<UpdateProfessor>
{
    public UpdateProfessorValidator()
    {
        _ = RuleFor(x => x.FirstName).NotEmpty();
        _ = RuleFor(x => x.LastName).NotEmpty();
        _ = RuleFor(x => x.EmailAddress).NotEmpty();
        _ = RuleFor(x => x.Rank).IsInEnum();
        _ = RuleFor(x => x.ProfessorId)
            .NotEmpty()
            .MustAsync(async (professorId, ct) =>
            {
                var professorRepo = Resolve<IProfessorRepository>();
                var professor = await professorRepo.GetByIdAsync(professorId);
                return professor is not null;
            })
            .WithMessage("Professor not found")
            .MustAsync(async (professorId, ct) =>
            {
                var professorRepo = Resolve<IProfessorRepository>();
                var universityRepo = Resolve<IUniversityRepository>();
                var userContext = Resolve<IUserContextService>();

                var professor = await professorRepo.GetByIdAsync(professorId);
                var university = await universityRepo.GetByIdAsync(professor.WorkPlace);
                var currentUser = userContext.GetCurrentUser();

                var presidentId = PresidentId.From(currentUser.Id.Value);
                return university.President == presidentId;
            })
            .WithMessage("You are not authorized to update this professor");
    }
}
