using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Aggregates.Users;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using Qowaiv;

namespace AcademicManagement.Application.UseCases.Professors;

public class CreateProfessorEndpoint : Endpoint<CreateProfessor, ProfessorId>
{
    public override void Configure()
    {
        Post("academic-management/professor/create");
        Policies(PolicyAcademicManagement.PresidentOnly);
    }

    public override async Task HandleAsync(CreateProfessor req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record CreateProfessor : ICommand<ProfessorId>
{
    public required UserName UserName { get; init; }
    public required Name FirstName { get; init; }
    public required Name LastName { get; init; }
    public required EmailAddress EmailAddress { get; init; }
    public required Rank Rank { get; init; }
    public required UniversityId WorkPlace { get; init; }
    public DepartmentId? DepartmentId { get; init; }
}

public class CreateProfessorHandler : ICommandHandler<CreateProfessor, ProfessorId>
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CreateProfessorHandler(IProfessorRepository professorRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IUniversityRepository universityRepository, IDepartmentRepository departmentRepository, IUserContextService userContextService)
    {
        _professorRepository = professorRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _universityRepository = universityRepository;
        _departmentRepository = departmentRepository;
        _userContextService = userContextService;
    }

    public async Task<ProfessorId> ExecuteAsync(CreateProfessor command, CancellationToken ct)
    {
        var university = await _universityRepository.GetByIdAsync(command.WorkPlace);

        var presidentId = _userContextService.GetPresidentId();
        if (university.President != presidentId)
        {
            throw new UnauthorizedAccessException("You are not authorized to create professors for this university");
        }

        var department = command.DepartmentId is not null ? await _departmentRepository.GetByIdAsync(command.DepartmentId.Value) : null;

        var user = User.Create(command.UserName, UserRole.Professor);
        _userRepository.Insert(user);

        var professor = ProfessorFactory.Create(
            university,
            department,
            command.FirstName,
            command.LastName,
            command.EmailAddress,
            command.Rank,
            user.Id
        );
        _professorRepository.Insert(professor);
        await _unitOfWork.SaveChangesAsync();
        return professor.Id;
    }
}
